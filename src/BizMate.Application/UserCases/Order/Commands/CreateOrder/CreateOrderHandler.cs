using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _DealerLevel = BizMate.Domain.Entities.DealerLevel;
using _Order = BizMate.Domain.Entities.Order;

namespace BizMate.Application.UserCases.Order.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IDealerLevelRepository _dealerLevelRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly IProductItemRepository _productItemRepository;
        private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
        private readonly ITechnicianHoldingRepository _technicianHoldingRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(
            IDealerLevelRepository dealerLevelRepository,
            ICustomerRepository customerRepository,
            IStockRepository stockRepository,
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            IProductItemRepository productItemRepository,
            IInventoryTransactionRepository inventoryTransactionRepository,
            ITechnicianHoldingRepository technicianHoldingRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            ILogger<CreateOrderHandler> logger)
        {
            _dealerLevelRepository = dealerLevelRepository;
            _customerRepository = customerRepository;
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _inventoryTransactionRepository = inventoryTransactionRepository;
            _technicianHoldingRepository = technicianHoldingRepository;
            _statusRepository = statusRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var storeId = _userSession.StoreId;
                var userId = Guid.Parse(_userSession.UserId);
                var requestDetails = request.Details.ToList();
                var technicianIds = NormalizeTechnicianIds(request.TechnicianIds, request.TechnicianId);
                await ValidateTechniciansAsync(storeId, technicianIds, cancellationToken);

                var orderId = Guid.NewGuid();
                var statusCode = request.IsDraft ? "DRAFT" : "NEW";
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(statusCode, "Order");
                if (statusId == Guid.Empty)
                    throw new InvalidOperationException("Trạng thái đơn hàng không hợp lệ.");

                var code = await _codeGeneratorService.GenerateCodeAsync("#DH", 5);
                var customer = request.CustomerId != null
                    ? await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken)
                    : null;

                var productIds = requestDetails.Select(d => d.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);

                var productItemsBySerial = await ValidateSerialNumbersAsync(
                    requestDetails,
                    productDict,
                    storeId,
                    cancellationToken);

                _DealerLevel? dealerLevel = null;
                if (request.CustomerType == 2 && customer?.DealerLevelId != null)
                    dealerLevel = await _dealerLevelRepository.GetByIdAsync(customer.DealerLevelId.Value, cancellationToken);

                var newOrder = new _Order
                {
                    Id = orderId,
                    Code = code,
                    StoreId = storeId,
                    CreatedBy = userId,
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    CustomerType = request.CustomerType,
                    DeliveryAddress = request.DeliveryAddress,
                    TechnicianId = technicianIds.FirstOrDefault() == Guid.Empty ? null : technicianIds.First(),
                    InstallationDate = request.InstallationDate.HasValue
                    ? DateTime.SpecifyKind(request.InstallationDate.Value, DateTimeKind.Utc)
                    : null,
                    StatusId = statusId,
                    Description = request.Description,
                    OrderTechnicians = BuildOrderTechnicians(orderId, technicianIds),
                    Details = BuildOrderDetails(
                        orderId,
                        requestDetails,
                        productDict,
                        dealerLevel,
                        request.CustomerType)
                };

                newOrder.RecalculateTotal();

                await _orderRepository.AddAsync(newOrder, cancellationToken);
                await ReserveStockAsync(storeId, newOrder.Details, userId, cancellationToken);
                await ReserveSerialItemsAsync(
                    newOrder,
                    requestDetails,
                    productDict,
                    productItemsBySerial,
                    userId,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return new CreateOrderResponse(true, "Tạo đơn hàng thành công.");
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, ex.Message);
                return new CreateOrderResponse(false, ex.Message);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng.");
                return new CreateOrderResponse(false, "Không thể tạo đơn hàng. Vui lòng thử lại.");
            }
        }

        private async Task ValidateTechniciansAsync(
            Guid storeId,
            IReadOnlyList<Guid> technicianIds,
            CancellationToken cancellationToken)
        {
            if (technicianIds.Count == 0)
                return;

            var technicians = await _technicianHoldingRepository.GetTechniciansByIdsAsync(
                storeId,
                technicianIds,
                cancellationToken);

            if (technicians.Count != technicianIds.Count)
                throw new InvalidOperationException("Danh sach ky thuat co nguoi khong ton tai trong store hien tai.");

            if (technicians.Any(x => !x.IsActive))
                throw new InvalidOperationException("Danh sach ky thuat co nguoi dang ngung hoat dong.");
        }

        private static List<Guid> NormalizeTechnicianIds(
            IEnumerable<Guid>? technicianIds,
            Guid? technicianId)
        {
            var result = new List<Guid>();
            var ids = technicianIds?.ToList();

            if (ids is { Count: > 0 })
            {
                foreach (var id in ids)
                    Add(id);

                return result;
            }

            if (technicianId.HasValue)
                Add(technicianId.Value);

            return result;

            void Add(Guid id)
            {
                if (id == Guid.Empty)
                    throw new InvalidOperationException("TechnicianId khong hop le.");

                if (!result.Contains(id))
                    result.Add(id);
            }
        }

        private static List<OrderTechnician> BuildOrderTechnicians(
            Guid orderId,
            IReadOnlyList<Guid> technicianIds)
        {
            var now = DateTime.UtcNow;
            return technicianIds
                .Select(technicianId => new OrderTechnician
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    TechnicianId = technicianId,
                    AssignedAt = now,
                    CreatedDate = now
                })
                .ToList();
        }

        public async Task ReserveStockAsync(
            Guid storeId,
            IEnumerable<OrderDetail> orderDetails,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var details = orderDetails.ToList();
            var productIds = details.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var detailGroup in details.GroupBy(d => d.ProductId))
            {
                if (!stockDict.TryGetValue(detailGroup.Key, out var stock))
                    throw new InvalidOperationException($"Không tìm thấy tồn kho cho sản phẩm  {detailGroup.Key}.");

                var quantity = detailGroup.Sum(x => x.Quantity);
                if (stock.Available < quantity)
                    throw new InvalidOperationException($"Sản phẩm {detailGroup.Key} không dư tồn. Khả dụng {stock.Available}, cần {quantity}.");

                stock.Reserved += quantity;
                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

        private static List<OrderDetail> BuildOrderDetails(
            Guid orderId,
            IReadOnlyList<CreateOrderDetailRequest> requestDetails,
            IReadOnlyDictionary<Guid, Product> productDict,
            _DealerLevel? dealerLevel,
            int customerType)
        {
            return requestDetails.Select(d =>
            {
                if (!productDict.TryGetValue(d.ProductId, out var product))
                    throw new InvalidOperationException($"Sản phẩm {d.ProductId} không tồn tại.");

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
                    Quantity = d.Quantity,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    UnitPrice = unitPrice,
                    Unit = product.Unit,
                    Total = d.Quantity * unitPrice
                };
            }).ToList();
        }

        private async Task<Dictionary<string, ProductItem>> ValidateSerialNumbersAsync(
            IReadOnlyList<CreateOrderDetailRequest> details,
            IReadOnlyDictionary<Guid, Product> productDict,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            if (details.Count == 0)
                throw new InvalidOperationException("Don hang phai co it nhat mot dong san pham.");

            var allSerials = new List<string>();
            var serialsByProductId = new Dictionary<Guid, List<string>>();

            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"Số lượng sản phẩm {detail.ProductId} phải lớn hơn 0.");

                if (!productDict.TryGetValue(detail.ProductId, out var product))
                    throw new InvalidOperationException($"Sản phẩm {detail.ProductId} không tồn tại.");

                var duplicates = SerialNumberNormalizer.FindDuplicates(detail.SerialNumbers);
                if (duplicates.Count > 0)
                    throw new InvalidOperationException($"SN bi trung trong dong san pham {product.Code}: {string.Join(", ", duplicates)}.");

                var serials = SerialNumberNormalizer.Normalize(detail.SerialNumbers);

                if (product.IsSerialTracked && serials.Count != detail.Quantity)
                    throw new InvalidOperationException($"San pham {product.Code} can dung {detail.Quantity} SN de tao don.");

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

                    if (item.Status != ProductItemStatus.InStock)
                        throw new InvalidOperationException(GetUnavailableSerialMessage(item, serial));
                }
            }

            return productItemsBySerial;
        }

        private async Task ReserveSerialItemsAsync(
            _Order order,
            IReadOnlyList<CreateOrderDetailRequest> requestDetails,
            IReadOnlyDictionary<Guid, Product> productDict,
            IReadOnlyDictionary<string, ProductItem> productItemsBySerial,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var orderDetails = order.Details.ToList();
            var itemsToUpdate = new List<ProductItem>();
            var transactions = new List<InventoryTransaction>();
            var now = DateTime.UtcNow;

            for (var i = 0; i < requestDetails.Count; i++)
            {
                var requestDetail = requestDetails[i];
                var product = productDict[requestDetail.ProductId];

                if (!product.IsSerialTracked)
                    continue;

                var orderDetail = orderDetails[i];
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
                        Note = $"Order {order.Code}"
                    });
                }
            }

            if (itemsToUpdate.Count == 0)
                return;

            await _productItemRepository.UpdateRangeAsync(itemsToUpdate, cancellationToken);
            await _inventoryTransactionRepository.AddRangeAsync(transactions, cancellationToken);
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
