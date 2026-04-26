using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.ExportReceipt.Commands.CreateExportReceipt;
using BizMate.Domain.Entities;
using BizMate.Public.Message;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Application.Common.Dto.CoreDto.OrderCoreDto;

namespace BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder
{
    public class UpdateStatusOrderHandler : IRequestHandler<UpdateStatusOrderRequest, UpdateStatusOrderResponse>
    {
        private readonly IMediator _mediator;
        private readonly IStatusRepository _statusRepository;
        private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserSession _userSession;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateStatusOrderHandler> _logger;

        public UpdateStatusOrderHandler(
            IOrderStatusHistoryRepository orderStatusHistoryRepository,
            IMediator mediator,
            IStockRepository stockRepository,
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            IStatusRepository statusRepository,
            IUserSession userSession,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            ILogger<UpdateStatusOrderHandler> logger)
        {
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
            _mediator = mediator;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _statusRepository = statusRepository;
            _userSession = userSession;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<UpdateStatusOrderResponse> Handle(
            UpdateStatusOrderRequest request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var storeId = _userSession.StoreId;
                var userId = Guid.Parse(_userSession.UserId);

                var order = await _orderRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
                if (order == null)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist, cancellationToken);

                if (order.RowVersion != request.RowVersion)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.NotValidRowversion, cancellationToken);

                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(request.StatusCode, "Order");
                if (statusId == Guid.Empty)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist2, cancellationToken);

                var currentStatus = await _statusRepository.GetIdById(request.StatusId, cancellationToken);
                var updateStatus = await _statusRepository.GetIdById(statusId, cancellationToken);
                if (currentStatus == null || updateStatus == null)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist2, cancellationToken);

                if (currentStatus.Code is "CANCELLED" or "COMPLETED")
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.RoleWithoutAuthority, cancellationToken);

                if (updateStatus.Code == "PACKED")
                {
                    await ReserveSerialItemsForOrderAsync(order, request.Details, storeId, userId, cancellationToken);
                }

                if (updateStatus.Code == "CANCELLED")
                {
                    await ReleaseSerialItemsAsync(order.Details, userId, cancellationToken);
                    await ReleaseReservedStockAsync(
                        storeId,
                        order.Details.Select(d => new OrderDetail
                        {
                            ProductId = d.ProductId,
                            Quantity = d.Quantity
                        }),
                        userId,
                        cancellationToken);
                }

                if (currentStatus.Code == "PACKED" && updateStatus.Code == "COMPLETED")
                {
                    await CompleteSerialItemsForOrderAsync(order, request.Details, storeId, userId, cancellationToken);
                    await CreateExportReceipt(order, cancellationToken);
                    await DeductStockAsync(storeId, order.Details, userId, cancellationToken);
                }

                var statusOrder = new UpdateOrderStatusDto
                {
                    Id = request.Id,
                    RowVersion = Guid.NewGuid(),
                    StatusId = statusId,
                    StoreId = storeId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                };

                await _orderRepository.UpdateStatusAsync(statusOrder, cancellationToken);

                await _orderStatusHistoryRepository.UpdateOrderStatus(
                    request.Id,
                    statusId,
                    userId,
                    _userSession.UserName ?? "Unknown",
                    "Đổi trạng thái từ " + currentStatus.Name + " thành " + updateStatus.Name
                );

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new UpdateStatusOrderResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, ex.Message);
                return new UpdateStatusOrderResponse(false, ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái phiếu nhập kho.");
                return new UpdateStatusOrderResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }

        public async Task DeductStockAsync(
            Guid storeId,
            IEnumerable<OrderDetailDto> orderDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var productIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var detail in orderDetails)
            {
                if (!stockDict.TryGetValue(detail.ProductId, out var stock))
                    throw new InvalidOperationException($"Không tìm thấy tồn kho cho sản phẩm {detail.ProductId}");

                if (stock.Reserved < detail.Quantity)
                    throw new InvalidOperationException($"Dữ liệu lệch: Reserved {stock.Reserved} nhỏ hơn cần xuất {detail.Quantity}");

                stock.Quantity -= detail.Quantity;
                stock.Reserved -= detail.Quantity;

                if (stock.Quantity < 0)
                    throw new InvalidOperationException($"Tồn kho âm cho sản phẩm {detail.ProductId}");

                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

        public async Task ReleaseReservedStockAsync(
            Guid storeId,
            IEnumerable<OrderDetail> orderDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var productIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var detail in orderDetails)
            {
                if (!stockDict.TryGetValue(detail.ProductId, out var stock))
                    continue;

                stock.Reserved -= detail.Quantity;
                if (stock.Reserved < 0)
                    stock.Reserved = 0;

                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

        private async Task<UpdateStatusOrderResponse> RollbackResponseAsync(
            string message,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            _logger.LogWarning(message);
            return new UpdateStatusOrderResponse(false, message);
        }

        private async Task CreateExportReceipt(_Order order, CancellationToken cancellationToken)
        {
            var request = new CreateExportReceiptRequest
            {
                CustomerName = order.CustomerName,
                CustomerPhone = order.CustomerPhone,
                DeliveryAddress = order.DeliveryAddress,
                Description = order.Description,
                Details = order.Details.Select(d => new CreateExportReceiptDetail
                {
                    ProductId = d.ProductId,
                    Quantity = d.Quantity
                }).ToList()
            };

            var response = await _mediator.Send(request, cancellationToken);
            if (!response.Success)
            {
                _logger.LogError("Không thể tạo phiếu xuất cho đơn hàng {OrderId}: {Message}", order.Id, response.Message);
                throw new Exception("Tạo phiếu xuất thất bại.");
            }
        }

        private async Task ReserveSerialItemsForOrderAsync(
            _Order order,
            IEnumerable<UpdateStatusOrderSerialDetail> requestDetails,
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var serialMap = BuildSerialMapByOrderDetail(order.Details, requestDetails);
            var products = await GetProductsAsync(order.Details, cancellationToken);
            var allSerials = serialMap.Values.SelectMany(x => x).ToList();
            var items = await _productItemRepository.GetBySerialNumbersAsync(allSerials, cancellationToken);
            var itemBySerial = items.ToDictionary(x => x.SerialNumber, StringComparer.OrdinalIgnoreCase);

            ValidateRequiredSerials(order.Details, products, serialMap);

            var now = DateTime.UtcNow;
            var itemsToUpdate = new List<ProductItem>();
            var transactions = new List<InventoryTransaction>();

            foreach (var detail in order.Details)
            {
                if (!products[detail.ProductId].IsSerialTracked)
                    continue;

                foreach (var serial in serialMap[detail.Id])
                {
                    var item = GetValidatedItem(itemBySerial, serial, detail, storeId, ProductItemStatus.InStock);
                    var fromStatus = item.Status;
                    item.Status = ProductItemStatus.Reserved;
                    item.OrderDetailId = detail.Id;
                    item.UpdatedBy = userId;
                    item.UpdatedDate = now;

                    itemsToUpdate.Add(item);
                    transactions.Add(CreateTransaction(item, InventoryTransactionType.Reserve, fromStatus, ProductItemStatus.Reserved, userId, now, $"Order {order.Code}"));
                }
            }

            if (itemsToUpdate.Count == 0)
                return;

            await _productItemRepository.UpdateRangeAsync(itemsToUpdate, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }

        private async Task CompleteSerialItemsForOrderAsync(
            _Order order,
            IEnumerable<UpdateStatusOrderSerialDetail> requestDetails,
            Guid storeId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var products = await GetProductsAsync(order.Details, cancellationToken);
            var serialMap = BuildSerialMapByOrderDetail(order.Details, requestDetails);
            var orderDetailIds = order.Details.Select(x => x.Id).ToList();
            var reservedItems = await _productItemRepository.GetByOrderDetailIdsAsync(
                orderDetailIds,
                ProductItemStatus.Reserved,
                cancellationToken);

            var allRequestedSerials = serialMap.Values.SelectMany(x => x).ToList();
            var requestedItems = await _productItemRepository.GetBySerialNumbersAsync(allRequestedSerials, cancellationToken);
            var requestedItemBySerial = requestedItems.ToDictionary(x => x.SerialNumber, StringComparer.OrdinalIgnoreCase);
            var reservedByDetailId = reservedItems
                .Where(x => x.OrderDetailId.HasValue)
                .GroupBy(x => x.OrderDetailId!.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            var now = DateTime.UtcNow;
            var itemsToUpdate = new List<ProductItem>();
            var transactions = new List<InventoryTransaction>();

            foreach (var detail in order.Details)
            {
                if (!products[detail.ProductId].IsSerialTracked)
                    continue;

                var detailReservedItems = reservedByDetailId.GetValueOrDefault(detail.Id) ?? new List<ProductItem>();
                var selectedItems = new List<ProductItem>();

                if (detailReservedItems.Count == detail.Quantity)
                {
                    selectedItems.AddRange(detailReservedItems);
                }
                else
                {
                    if (detailReservedItems.Count > 0)
                        throw new InvalidOperationException($"Du lieu SN cua san pham {detail.ProductCode} khong khop so luong xuat.");

                    if (!serialMap.TryGetValue(detail.Id, out var serials) || serials.Count != detail.Quantity)
                        throw new InvalidOperationException($"San pham {detail.ProductCode} can {detail.Quantity} SN de xuat kho.");

                    selectedItems.AddRange(serials.Select(serial =>
                        GetValidatedItem(requestedItemBySerial, serial, detail, storeId, ProductItemStatus.InStock)));
                }

                foreach (var item in selectedItems)
                {
                    var fromStatus = item.Status;
                    item.Status = ProductItemStatus.Sold;
                    item.OrderDetailId = detail.Id;
                    item.SoldAt = now;
                    item.UpdatedBy = userId;
                    item.UpdatedDate = now;

                    itemsToUpdate.Add(item);
                    transactions.Add(CreateTransaction(item, InventoryTransactionType.Export, fromStatus, ProductItemStatus.Sold, userId, now, $"Order {order.Code}"));
                }
            }

            if (itemsToUpdate.Count == 0)
                return;

            await _productItemRepository.UpdateRangeAsync(itemsToUpdate, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }

        private async Task ReleaseSerialItemsAsync(
            IEnumerable<OrderDetailDto> orderDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var reservedItems = await _productItemRepository.GetByOrderDetailIdsAsync(
                orderDetails.Select(x => x.Id),
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

                transactions.Add(CreateTransaction(item, InventoryTransactionType.Release, fromStatus, ProductItemStatus.InStock, userId, now, "Order cancelled"));
            }

            await _productItemRepository.UpdateRangeAsync(reservedItems, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
        }

        private Dictionary<Guid, List<string>> BuildSerialMapByOrderDetail(
            IEnumerable<OrderDetailDto> orderDetails,
            IEnumerable<UpdateStatusOrderSerialDetail> requestDetails)
        {
            var orderDetailsList = orderDetails.ToList();
            var serialMap = new Dictionary<Guid, List<string>>();
            var allSerials = new List<string>();

            foreach (var requestDetail in requestDetails)
            {
                var serials = SerialNumberNormalizer.Normalize(requestDetail.SerialNumbers);
                if (serials.Count == 0)
                    continue;

                var duplicates = SerialNumberNormalizer.FindDuplicates(requestDetail.SerialNumbers);
                if (duplicates.Count > 0)
                    throw new InvalidOperationException($"SN bi trung: {string.Join(", ", duplicates)}");

                var orderDetail = ResolveOrderDetail(orderDetailsList, requestDetail);
                if (serialMap.ContainsKey(orderDetail.Id))
                    throw new InvalidOperationException($"Dong san pham {orderDetail.ProductCode} bi gui SN nhieu lan.");

                serialMap[orderDetail.Id] = serials;
                allSerials.AddRange(serials);
            }

            var duplicateAcrossDetails = allSerials
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateAcrossDetails.Count > 0)
                throw new InvalidOperationException($"SN bi trung trong don hang: {string.Join(", ", duplicateAcrossDetails)}");

            return serialMap;
        }

        private static OrderDetailDto ResolveOrderDetail(
            IEnumerable<OrderDetailDto> orderDetails,
            UpdateStatusOrderSerialDetail requestDetail)
        {
            if (requestDetail.OrderDetailId.HasValue)
            {
                return orderDetails.FirstOrDefault(x => x.Id == requestDetail.OrderDetailId.Value)
                    ?? throw new InvalidOperationException($"Order detail {requestDetail.OrderDetailId.Value} khong ton tai.");
            }

            var matches = orderDetails.Where(x => x.ProductId == requestDetail.ProductId).ToList();
            if (matches.Count == 1)
                return matches[0];

            if (matches.Count == 0)
                throw new InvalidOperationException($"San pham {requestDetail.ProductId} khong co trong don hang.");

            throw new InvalidOperationException($"San pham {requestDetail.ProductId} co nhieu dong, can gui OrderDetailId.");
        }

        private static void ValidateRequiredSerials(
            IEnumerable<OrderDetailDto> orderDetails,
            IReadOnlyDictionary<Guid, Product> products,
            IReadOnlyDictionary<Guid, List<string>> serialMap)
        {
            foreach (var detail in orderDetails)
            {
                if (!products[detail.ProductId].IsSerialTracked)
                    continue;

                if (!serialMap.TryGetValue(detail.Id, out var serials) || serials.Count != detail.Quantity)
                    throw new InvalidOperationException($"San pham {detail.ProductCode} can {detail.Quantity} SN.");
            }
        }

        private async Task<Dictionary<Guid, Product>> GetProductsAsync(
            IEnumerable<OrderDetailDto> details,
            CancellationToken cancellationToken)
        {
            var productIds = details.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
            var productDict = products.ToDictionary(x => x.Id);

            foreach (var productId in productIds)
            {
                if (!productDict.ContainsKey(productId))
                    throw new InvalidOperationException($"San pham {productId} khong ton tai.");
            }

            return productDict;
        }

        private static ProductItem GetValidatedItem(
            IReadOnlyDictionary<string, ProductItem> itemBySerial,
            string serial,
            OrderDetailDto detail,
            Guid storeId,
            ProductItemStatus expectedStatus)
        {
            if (!itemBySerial.TryGetValue(serial, out var item))
                throw new InvalidOperationException($"SN {serial} khong ton tai.");

            if (item.StoreId != storeId)
                throw new InvalidOperationException($"SN {serial} khong thuoc cua hang hien tai.");

            if (item.ProductId != detail.ProductId)
                throw new InvalidOperationException($"SN {serial} khong thuoc san pham {detail.ProductCode}.");

            if (item.Status != expectedStatus)
                throw new InvalidOperationException($"SN {serial} khong o trang thai hop le.");

            return item;
        }

        private static InventoryTransaction CreateTransaction(
            ProductItem item,
            InventoryTransactionType type,
            ProductItemStatus fromStatus,
            ProductItemStatus toStatus,
            Guid userId,
            DateTime now,
            string note)
        {
            return new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                ProductItemId = item.Id,
                Type = type,
                FromStatus = fromStatus,
                ToStatus = toStatus,
                CreatedBy = userId,
                CreatedDate = now,
                Note = note
            };
        }
    }
}
