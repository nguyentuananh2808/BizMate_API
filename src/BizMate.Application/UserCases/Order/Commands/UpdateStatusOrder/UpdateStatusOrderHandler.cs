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

                if (order.StatusId != request.StatusId)
                    return await RollbackResponseAsync("Trạng thái đơn hàng đã thay đổi, vui lòng tải lại dữ liệu.", cancellationToken);

                var currentStatus = await _statusRepository.GetIdById(order.StatusId, cancellationToken);
                var updateStatus = await _statusRepository.GetIdById(statusId, cancellationToken);
                if (currentStatus == null || updateStatus == null)
                    return await RollbackResponseAsync(ValidationMessage.LocalizedStrings.DataNotExist2, cancellationToken);

                if (currentStatus.Id == updateStatus.Id)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new UpdateStatusOrderResponse(true, "Trạng thái đơn hàng không thay đổi.");
                }

                if (currentStatus.Code == "COMPLETED")
                    return await RollbackResponseAsync("Đơn hàng đã hoàn thành, chỉ được cập nhật mô tả ở màn hình cập nhật đơn.", cancellationToken);

                if (updateStatus.Code is not ("NEW" or "COMPLETED"))
                    return await RollbackResponseAsync("Đơn hàng chỉ còn hỗ trợ trạng thái Nháp, Tạo mới và Hoàn thành.", cancellationToken);

                if (updateStatus.Code == "NEW" && currentStatus.Code != "DRAFT")
                    return await RollbackResponseAsync("Chỉ đơn nháp mới được chuyển sang tạo mới.", cancellationToken);

                if (updateStatus.Code == "COMPLETED" && currentStatus.Code != "NEW")
                    return await RollbackResponseAsync("Đơn hàng phải ở trạng thái Tạo mới trước khi hoàn thành.", cancellationToken);

                if (updateStatus.Code == "COMPLETED")
                {
                    await CreateExportReceipt(order, cancellationToken);

                    if (!order.TechnicianExportedAt.HasValue)
                    {
                        await CompleteSerialItemsForOrderAsync(order, request.Details, storeId, userId, cancellationToken);
                        await DeductStockAsync(storeId, order.Details, userId, cancellationToken);
                    }
                }

                var simplifiedStatusOrder = new UpdateOrderStatusDto
                {
                    Id = request.Id,
                    RowVersion = Guid.NewGuid(),
                    StatusId = statusId,
                    StoreId = storeId,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.UtcNow
                };

                await _orderRepository.UpdateStatusAsync(simplifiedStatusOrder, cancellationToken);

                await _orderStatusHistoryRepository.AddOrderStatusHistory(
                    request.Id,
                    currentStatus.Id,
                    statusId,
                    userId,
                    _userSession.UserName ?? "Unknown",
                    "Đổi trạng thái từ " + currentStatus.Name + " thành " + updateStatus.Name
                );

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return new UpdateStatusOrderResponse(true, "Cập nhật trạng thái đơn hàng thành công.");
            }
            catch (InvalidOperationException ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogWarning(ex, "Không thể cập nhật trạng thái đơn hàng do dữ liệu nghiệp vụ không hợp lệ.");
                return new UpdateStatusOrderResponse(
                    false,
                    "Không thể cập nhật trạng thái đơn hàng. Vui lòng kiểm tra tồn kho, serial và tải lại dữ liệu rồi thử lại.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng.");
                return new UpdateStatusOrderResponse(false, "Không thể cập nhật trạng thái đơn hàng. Vui lòng thử lại.");
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
                Details = order.Details
                    .Select(d => new CreateExportReceiptDetail
                    {
                        ProductId = d.ProductId,
                        Quantity = d.Quantity + d.UsedBorrowedQuantity
                    })
                    .Where(d => d.Quantity > 0)
                    .ToList()
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
            var orderDetailIds = order.Details.Select(x => x.Id).ToList();
            var existingReservedItems = await _productItemRepository.GetByOrderDetailIdsAsync(
                orderDetailIds,
                ProductItemStatus.Reserved,
                cancellationToken);

            var reservedByDetailId = existingReservedItems
                .Where(x => x.OrderDetailId.HasValue)
                .GroupBy(x => x.OrderDetailId!.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            var serialsToReserve = new Dictionary<Guid, List<string>>();

            var now = DateTime.UtcNow;
            var itemsToUpdate = new List<ProductItem>();
            var transactions = new List<InventoryTransaction>();

            foreach (var detail in order.Details)
            {
                if (!products[detail.ProductId].IsSerialTracked)
                {
                    if (serialMap.TryGetValue(detail.Id, out var ignoredSerials) && ignoredSerials.Count > 0)
                        throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} không bật quản lý SN.");

                    continue;
                }

                var existingReservedForDetail = reservedByDetailId.GetValueOrDefault(detail.Id) ?? new List<ProductItem>();
                if (existingReservedForDetail.Count == detail.Quantity)
                {
                    if (serialMap.TryGetValue(detail.Id, out var requestedSerials) && requestedSerials.Count > 0)
                        EnsureRequestedSerialsMatchReserved(detail, requestedSerials, existingReservedForDetail);

                    continue;
                }

                if (existingReservedForDetail.Count > 0)
                    throw new InvalidOperationException($"Dữ liệu SN của sản phẩm {detail.ProductCode} không khớp số lượng giữ chỗ.");

                if (!serialMap.TryGetValue(detail.Id, out var serials) || serials.Count != detail.Quantity)
                    throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} cần {detail.Quantity} SN.");

                serialsToReserve[detail.Id] = serials;
            }

            var allSerials = serialsToReserve.Values.SelectMany(x => x).ToList();
            var items = await _productItemRepository.GetBySerialNumbersAsync(allSerials, cancellationToken);
            var itemBySerial = items.ToDictionary(x => x.SerialNumber, StringComparer.OrdinalIgnoreCase);

            foreach (var detail in order.Details)
            {
                if (!serialsToReserve.TryGetValue(detail.Id, out var serials))
                    continue;

                foreach (var serial in serials)
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
                {
                    if (serialMap.TryGetValue(detail.Id, out var ignoredSerials) && ignoredSerials.Count > 0)
                        throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} không bật quản lý SN.");

                    continue;
                }

                var detailReservedItems = reservedByDetailId.GetValueOrDefault(detail.Id) ?? new List<ProductItem>();
                var selectedItems = new List<ProductItem>();

                if (detailReservedItems.Count == detail.Quantity)
                {
                    if (serialMap.TryGetValue(detail.Id, out var requestedSerials) && requestedSerials.Count > 0)
                        EnsureRequestedSerialsMatchReserved(detail, requestedSerials, detailReservedItems);

                    selectedItems.AddRange(detailReservedItems);
                }
                else
                {
                    if (detailReservedItems.Count > 0)
                        throw new InvalidOperationException($"Dữ liệu SN của sản phẩm {detail.ProductCode} không khớp số lượng xuất.");

                    if (!serialMap.TryGetValue(detail.Id, out var serials) || serials.Count != detail.Quantity)
                        throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} cần {detail.Quantity} SN để xuất kho.");

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
                    throw new InvalidOperationException($"SN bị trùng: {string.Join(", ", duplicates)}");

                var orderDetail = ResolveOrderDetail(orderDetailsList, requestDetail);
                if (serialMap.ContainsKey(orderDetail.Id))
                    throw new InvalidOperationException($"Dòng sản phẩm {orderDetail.ProductCode} bị gửi SN nhiều lần.");

                serialMap[orderDetail.Id] = serials;
                allSerials.AddRange(serials);
            }

            var duplicateAcrossDetails = allSerials
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (duplicateAcrossDetails.Count > 0)
                throw new InvalidOperationException($"SN bị trùng trong đơn hàng: {string.Join(", ", duplicateAcrossDetails)}");

            return serialMap;
        }

        private static OrderDetailDto ResolveOrderDetail(
            IEnumerable<OrderDetailDto> orderDetails,
            UpdateStatusOrderSerialDetail requestDetail)
        {
            if (requestDetail.OrderDetailId.HasValue)
            {
                return orderDetails.FirstOrDefault(x => x.Id == requestDetail.OrderDetailId.Value)
                    ?? throw new InvalidOperationException($"Dòng đơn hàng {requestDetail.OrderDetailId.Value} không tồn tại.");
            }

            var matches = orderDetails.Where(x => x.ProductId == requestDetail.ProductId).ToList();
            if (matches.Count == 1)
                return matches[0];

            if (matches.Count == 0)
                throw new InvalidOperationException($"Sản phẩm {requestDetail.ProductId} không có trong đơn hàng.");

            throw new InvalidOperationException($"Sản phẩm {requestDetail.ProductId} có nhiều dòng, cần gửi OrderDetailId.");
        }

        private static void ValidateRequiredSerials(
            IEnumerable<OrderDetailDto> orderDetails,
            IReadOnlyDictionary<Guid, Product> products,
            IReadOnlyDictionary<Guid, List<string>> serialMap)
        {
            foreach (var detail in orderDetails)
            {
                if (!products[detail.ProductId].IsSerialTracked)
                {
                    if (serialMap.TryGetValue(detail.Id, out var ignoredSerials) && ignoredSerials.Count > 0)
                        throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} không bật quản lý SN.");

                    continue;
                }

                if (!serialMap.TryGetValue(detail.Id, out var serials) || serials.Count != detail.Quantity)
                    throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} cần {detail.Quantity} SN.");
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
                    throw new InvalidOperationException($"Sản phẩm {productId} không tồn tại.");
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
                throw new InvalidOperationException($"SN {serial} không tồn tại.");

            if (item.StoreId != storeId)
                throw new InvalidOperationException($"SN {serial} không thuộc cửa hàng hiện tại.");

            if (item.ProductId != detail.ProductId)
                throw new InvalidOperationException($"SN {serial} không thuộc sản phẩm {detail.ProductCode}.");

            if (item.Status != expectedStatus)
            {
                if (item.Status == ProductItemStatus.Reserved)
                {
                    var ownerMessage = item.OrderDetailId == detail.Id
                        ? "SN này đã được giữ cho dòng sản phẩm hiện tại."
                        : "SN này đã được giữ cho đơn hàng khác.";

                    throw new InvalidOperationException($"SN {serial} không khả dụng. {ownerMessage}");
                }

                throw new InvalidOperationException($"SN {serial} không khả dụng để xuất. Trạng thái hiện tại: {item.Status}.");
            }

            return item;
        }

        private static void EnsureRequestedSerialsMatchReserved(
            OrderDetailDto detail,
            IReadOnlyCollection<string> requestedSerials,
            IReadOnlyCollection<ProductItem> reservedItems)
        {
            if (requestedSerials.Count != detail.Quantity)
                throw new InvalidOperationException($"Sản phẩm {detail.ProductCode} cần đúng {detail.Quantity} SN.");

            var reservedSerials = reservedItems
                .Select(x => x.SerialNumber)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var invalidSerials = requestedSerials
                .Where(x => !reservedSerials.Contains(x))
                .ToList();

            if (invalidSerials.Count > 0)
                throw new InvalidOperationException($"SN đã giữ của sản phẩm {detail.ProductCode} không khớp: {string.Join(", ", invalidSerials)}.");
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
