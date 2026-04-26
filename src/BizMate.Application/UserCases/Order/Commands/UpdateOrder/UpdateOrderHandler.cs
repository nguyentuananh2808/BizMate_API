using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;
using _DealerLevel = BizMate.Domain.Entities.DealerLevel;
using _Order = BizMate.Application.Common.Dto.CoreDto.OrderCoreDto;
using _Status = BizMate.Domain.Entities.Status;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
    {
        private readonly IStatusRepository _statusRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateOrderHandler> _logger;
        private readonly IStockRepository _stockRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDealerLevelRepository _dealerLevelRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderHandler(
            IStatusRepository statusRepository,
            INotificationRepository notificationRepository,
            IProductRepository productRepository,
            IUserSession userSession,
            IOrderRepository orderRepository,
            ILogger<UpdateOrderHandler> logger,
            IStockRepository stockRepository,
            ICustomerRepository customerRepository,
            IDealerLevelRepository dealerLevelRepository,
            IProductItemRepository productItemRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IUnitOfWork unitOfWork)
        {
            _statusRepository = statusRepository;
            _notificationRepository = notificationRepository;
            _productRepository = productRepository;
            _userSession = userSession;
            _orderRepository = orderRepository;
            _logger = logger;
            _stockRepository = stockRepository;
            _customerRepository = customerRepository;
            _dealerLevelRepository = dealerLevelRepository;
            _productItemRepository = productItemRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateOrderResponse> Handle(UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_userSession.UserId);
            var storeId = _userSession.StoreId;

            var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (order == null)
                return NotFoundResponse(request.Id);

            var currentStatus = await _statusRepository.GetIdById(order.StatusId, cancellationToken);
            if (currentStatus == null)
                return new UpdateOrderResponse(false, "Trang thai hien tai khong hop le.");

            if (request.StatusId != Guid.Empty && request.StatusId != order.StatusId)
                return new UpdateOrderResponse(false, "Khong cap nhat trang thai o API nay. Hay dung endpoint update_status.");

            if (currentStatus.Code is "PACKED" or "COMPLETED" or "CANCELLED")
                return new UpdateOrderResponse(false, "Don hang da giu SN, hoan thanh hoac huy, khong the cap nhat chi tiet.");

            if (order.RowVersion != request.RowVersion)
                return new UpdateOrderResponse(false, "Du lieu da thay doi, vui long tai lai.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var requestDetails = request.Details.ToList();

                var customer = request.CustomerId != null
                    ? await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken)
                    : null;

                _DealerLevel? dealerLevel = null;
                if (request.CustomerType == 2 && customer?.DealerLevelId != null)
                    dealerLevel = await _dealerLevelRepository.GetByIdAsync(customer.DealerLevelId.Value, cancellationToken);

                var productIds = requestDetails.Select(d => d.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);
                var oldOrderDetailIds = order.Details.Select(x => x.Id).ToHashSet();

                var productItemsBySerial = await ValidateSerialNumbersAsync(
                    requestDetails,
                    productDict,
                    storeId,
                    oldOrderDetailIds,
                    cancellationToken);

                var newDetails = BuildOrderDetails(
                    order.Id,
                    requestDetails,
                    productDict,
                    dealerLevel,
                    request.CustomerType,
                    userId);

                await ReleaseReservedSerialItemsAsync(order.Details, userId, cancellationToken);

                UpdateOrderInfo(order, request, userId);
                await _orderRepository.UpdateOrderAsync(order, cancellationToken);
                await _orderRepository.UpdateWithDetailsAsync(order, newDetails, cancellationToken);

                await ReserveStockAsync(storeId, order.Details, newDetails, userId, cancellationToken);
                await ReserveSerialItemsAsync(
                    newDetails,
                    requestDetails,
                    productDict,
                    productItemsBySerial,
                    userId,
                    cancellationToken);

                await AddNotificationAsync(order, currentStatus, userId, storeId);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new UpdateOrderResponse(true, "Cap nhat don hang thanh cong.");
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, ex.Message);
                return new UpdateOrderResponse(false, ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Loi khi cap nhat don hang.");
                return new UpdateOrderResponse(false, "Khong the cap nhat don hang. Vui long thu lai.");
            }
        }

        public async Task ReserveStockAsync(
            Guid storeId,
            IEnumerable<OrderDetailDto> oldDetails,
            IEnumerable<OrderDetail> newDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var oldQuantities = oldDetails
                .GroupBy(d => d.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var newQuantities = newDetails
                .GroupBy(d => d.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var productIds = oldQuantities.Keys
                .Union(newQuantities.Keys)
                .Distinct()
                .ToList();

            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var productId in productIds)
            {
                var oldQty = oldQuantities.GetValueOrDefault(productId);
                var newQty = newQuantities.GetValueOrDefault(productId);
                var diff = newQty - oldQty;

                if (diff == 0)
                    continue;

                if (!stockDict.TryGetValue(productId, out var stock))
                    throw new InvalidOperationException($"Khong tim thay ton kho cho san pham {productId}.");

                if (diff > 0 && stock.Available < diff)
                    throw new InvalidOperationException($"San pham {productId} khong du ton. Kha dung {stock.Available}, can them {diff}.");

                if (stock.Reserved + diff < 0)
                    throw new InvalidOperationException($"Du lieu ton kho lech cho san pham {productId}.");

                stock.Reserved += diff;
                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

        private static List<OrderDetail> BuildOrderDetails(
            Guid orderId,
            IReadOnlyList<UpdateOrderDetailRequest> requestDetails,
            IReadOnlyDictionary<Guid, Product> productDict,
            _DealerLevel? dealerLevel,
            int customerType,
            Guid userId)
        {
            return requestDetails.Select(d =>
            {
                if (!productDict.TryGetValue(d.ProductId, out var product))
                    throw new InvalidOperationException($"San pham {d.ProductId} khong ton tai.");

                var unitPrice = product.SalePrice.GetValueOrDefault(0);
                if (customerType == 2 && dealerLevel != null)
                {
                    var dealerPrice = dealerLevel.DealerPrices
                        .FirstOrDefault(dp => dp.ProductId == d.ProductId && !dp.IsDeleted);

                    unitPrice = dealerPrice?.Price ?? unitPrice;
                }

                return new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    ProductId = d.ProductId,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    Unit = product.Unit,
                    Quantity = d.Quantity,
                    UnitPrice = unitPrice,
                    Total = d.Quantity * unitPrice,
                    RowVersion = Guid.NewGuid(),
                    IsDeleted = false,
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };
            }).ToList();
        }

        private async Task<Dictionary<string, ProductItem>> ValidateSerialNumbersAsync(
            IReadOnlyList<UpdateOrderDetailRequest> details,
            IReadOnlyDictionary<Guid, Product> productDict,
            Guid storeId,
            IReadOnlySet<Guid> currentOrderDetailIds,
            CancellationToken cancellationToken)
        {
            if (details.Count == 0)
                throw new InvalidOperationException("Don hang phai co it nhat mot dong san pham.");

            var allSerials = new List<string>();
            var serialsByProductId = new Dictionary<Guid, List<string>>();

            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"So luong san pham {detail.ProductId} phai lon hon 0.");

                if (!productDict.TryGetValue(detail.ProductId, out var product))
                    throw new InvalidOperationException($"San pham {detail.ProductId} khong ton tai.");

                var duplicates = SerialNumberNormalizer.FindDuplicates(detail.SerialNumbers);
                if (duplicates.Count > 0)
                    throw new InvalidOperationException($"SN bi trung trong dong san pham {product.Code}: {string.Join(", ", duplicates)}.");

                var serials = SerialNumberNormalizer.Normalize(detail.SerialNumbers);

                if (product.IsSerialTracked && serials.Count != detail.Quantity)
                    throw new InvalidOperationException($"San pham {product.Code} can dung {detail.Quantity} SN de cap nhat don.");

                if (!product.IsSerialTracked && serials.Count > 0)
                    throw new InvalidOperationException($"San pham {product.Code} khong bat quan ly SN.");

                allSerials.AddRange(serials);

                if (!product.IsSerialTracked)
                    continue;

                if (!serialsByProductId.TryGetValue(detail.ProductId, out var productSerials))
                {
                    productSerials = new List<string>();
                    serialsByProductId[detail.ProductId] = productSerials;
                }

                productSerials.AddRange(serials);
            }

            var duplicateAcrossDetails = allSerials
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateAcrossDetails.Count > 0)
                throw new InvalidOperationException($"SN bi trung trong don hang: {string.Join(", ", duplicateAcrossDetails)}.");

            var productItems = await _productItemRepository.GetBySerialNumbersAsync(allSerials, cancellationToken);
            var productItemsBySerial = productItems.ToDictionary(x => x.SerialNumber, StringComparer.OrdinalIgnoreCase);

            foreach (var (productId, serials) in serialsByProductId)
            {
                var product = productDict[productId];

                foreach (var serial in serials)
                {
                    if (!productItemsBySerial.TryGetValue(serial, out var item))
                        throw new InvalidOperationException($"SN {serial} khong ton tai.");

                    if (item.StoreId != storeId)
                        throw new InvalidOperationException($"SN {serial} khong thuoc cua hang hien tai.");

                    if (item.ProductId != productId)
                        throw new InvalidOperationException($"SN {serial} khong thuoc san pham {product.Code}.");

                    if (item.Status == ProductItemStatus.InStock)
                        continue;

                    var isReservedForCurrentOrder = item.Status == ProductItemStatus.Reserved
                        && item.OrderDetailId.HasValue
                        && currentOrderDetailIds.Contains(item.OrderDetailId.Value);

                    if (isReservedForCurrentOrder)
                        continue;

                    throw new InvalidOperationException(GetUnavailableSerialMessage(item, serial));
                }
            }

            return productItemsBySerial;
        }

        private async Task ReleaseReservedSerialItemsAsync(
            IEnumerable<OrderDetailDto> oldDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var reservedItems = await _productItemRepository.GetByOrderDetailIdsAsync(
                oldDetails.Select(x => x.Id),
                ProductItemStatus.Reserved,
                cancellationToken);

            if (reservedItems.Count == 0)
                return;

            var now = DateTime.UtcNow;
            var transactions = new List<InventoryTransaction>();

            foreach (var item in reservedItems)
            {
                var fromStatus = item.Status;
                item.Status = ProductItemStatus.InStock;
                item.OrderDetailId = null;
                item.UpdatedBy = userId;
                item.UpdatedDate = now;

                transactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    ProductItemId = item.Id,
                    Type = InventoryTransactionType.Release,
                    FromStatus = fromStatus,
                    ToStatus = ProductItemStatus.InStock,
                    CreatedBy = userId,
                    CreatedDate = now,
                    Note = "Order updated"
                });
            }

            await _productItemRepository.UpdateRangeAsync(reservedItems, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }

        private async Task ReserveSerialItemsAsync(
            IReadOnlyList<OrderDetail> newDetails,
            IReadOnlyList<UpdateOrderDetailRequest> requestDetails,
            IReadOnlyDictionary<Guid, Product> productDict,
            IReadOnlyDictionary<string, ProductItem> productItemsBySerial,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var itemsToUpdate = new List<ProductItem>();
            var transactions = new List<InventoryTransaction>();
            var now = DateTime.UtcNow;

            for (var i = 0; i < requestDetails.Count; i++)
            {
                var requestDetail = requestDetails[i];
                var product = productDict[requestDetail.ProductId];

                if (!product.IsSerialTracked)
                    continue;

                var orderDetail = newDetails[i];
                var serials = SerialNumberNormalizer.Normalize(requestDetail.SerialNumbers);

                foreach (var serial in serials)
                {
                    var item = productItemsBySerial[serial];
                    var fromStatus = item.Status;

                    item.Status = ProductItemStatus.Reserved;
                    item.OrderDetailId = orderDetail.Id;
                    item.UpdatedBy = userId;
                    item.UpdatedDate = now;

                    itemsToUpdate.Add(item);
                    transactions.Add(new InventoryTransaction
                    {
                        Id = Guid.NewGuid(),
                        ProductItemId = item.Id,
                        Type = InventoryTransactionType.Reserve,
                        FromStatus = fromStatus,
                        ToStatus = ProductItemStatus.Reserved,
                        CreatedBy = userId,
                        CreatedDate = now,
                        Note = "Order updated"
                    });
                }
            }

            if (itemsToUpdate.Count == 0)
                return;

            await _productItemRepository.UpdateRangeAsync(itemsToUpdate, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }

        private static void UpdateOrderInfo(_Order order, UpdateOrderRequest request, Guid userId)
        {
            order.CustomerId = request.CustomerId;
            order.CustomerType = request.CustomerType;
            order.CustomerPhone = request.CustomerPhone;
            order.CustomerName = request.CustomerName;
            order.DeliveryAddress = request.DeliveryAddress;
            order.Description = request.Description;
            order.UpdatedDate = DateTime.UtcNow;
            order.UpdatedBy = userId;
            order.RowVersion = Guid.NewGuid();
            order.Status = null;
        }

        private UpdateOrderResponse NotFoundResponse(Guid id)
        {
            var message = ValidationMessage.LocalizedStrings.DataNotExist;
            _logger.LogWarning(message);
            return new UpdateOrderResponse(false, message);
        }

        private async Task AddNotificationAsync(_Order order, _Status status, Guid userId, Guid storeId)
        {
            var notif = new NotificationDto
            {
                UserId = null,
                OrderId = order.Id,
                StoreId = storeId,
                Message = $"{order.Id} | {order.Code}",
                Type = "order"
            };

            await _notificationRepository.BroadcastAsync(notif);
        }

        private static string GetUnavailableSerialMessage(ProductItem item, string serial)
        {
            if (item.Status == ProductItemStatus.Reserved)
            {
                return item.OrderDetailId.HasValue
                    ? $"SN {serial} da duoc giu cho don hang khac."
                    : $"SN {serial} dang o trang thai da giu cho.";
            }

            return $"SN {serial} khong kha dung de xuat. Trang thai hien tai: {item.Status}.";
        }
    }
}
