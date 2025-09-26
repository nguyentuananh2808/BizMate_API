using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Application.Common.Dto.CoreDto.OrderCoreDto;
using _Status = BizMate.Domain.Entities.Status;
using _DealerLevel = BizMate.Domain.Entities.DealerLevel;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Public.Message;

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

        public UpdateOrderHandler(
            IStatusRepository statusRepository,
            INotificationRepository notificationRepository,
            IProductRepository productRepository,
            IUserSession userSession,
            IOrderRepository orderRepository,
            ILogger<UpdateOrderHandler> logger,
            IStockRepository stockRepository,
            ICustomerRepository customerRepository,
            IDealerLevelRepository dealerLevelRepository)
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
        }

        public async Task<UpdateOrderResponse> Handle(UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_userSession.UserId);
            var storeId = _userSession.StoreId;

            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) return NotFoundResponse(request.Id);

            var currentStatus = await _statusRepository.GetIdById(order.StatusId, cancellationToken);
            if (currentStatus == null)
                return new UpdateOrderResponse(false, "Trạng thái hiện tại không hợp lệ");

            var newStatus = await _statusRepository.GetIdById(request.StatusId, cancellationToken);
            if (newStatus == null)
                return new UpdateOrderResponse(false, "Trạng thái mới không hợp lệ");

            if (currentStatus.Code is "COMPLETED" or "CANCELLED")
                return new UpdateOrderResponse(false, "Đơn hàng đã hoàn thành hoặc hủy, không thể cập nhật");

            if (order.RowVersion != request.RowVersion)
                return new UpdateOrderResponse(false, "Dữ liệu đã thay đổi, vui lòng tải lại");

            try
            {
                // 1️ Cập nhật thông tin đơn hàng
                UpdateOrderInfo(order, request, userId);

                await _orderRepository.UpdateOrderAsync(order, cancellationToken);

                // 2️ Validate tồn kho theo status
                var stockResult = await HandleStatusTransitionAsync(order, currentStatus, newStatus, userId, storeId, cancellationToken);
                if (!stockResult.Success)
                    return new UpdateOrderResponse(false, stockResult.ErrorMessage);

                // 3️ Lấy customer & dealer level nếu cần
                var customer = request.CustomerId != null
                    ? await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken)
                    : null;

                _DealerLevel? dealerLevel = null;
                if (request.CustomerType == 2 && customer?.DealerLevelId != null)
                    dealerLevel = await _dealerLevelRepository.GetByIdAsync(customer.DealerLevelId.Value, cancellationToken);

                // 4️ Build danh sách chi tiết mới
                var productIds = request.Details.Select(d => d.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);

                var newDetails = request.Details.Select(d =>
                {
                    var product = productDict[d.ProductId];
                    decimal unitPrice = product.SalePrice.GetValueOrDefault(0);

                    if (request.CustomerType == 2 && dealerLevel != null)
                    {
                        var dealerPrice = dealerLevel.DealerPrices
                            .FirstOrDefault(dp => dp.ProductId == d.ProductId && !dp.IsDeleted);
                        unitPrice = dealerPrice?.Price ?? unitPrice;
                    }

                    return new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        OrderId = order.Id,
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

                // 5️ Gọi repository cập nhật + thay thế chi tiết
                await _orderRepository.UpdateWithDetailsAsync(order, newDetails, cancellationToken);

                // 6️ Tính lại tổng đơn hàng
                order.RecalculateTotal();

                //7 cập nhật lại số lượng sản phẩm giữ chỗ 
                await ReserveStockAsync(
                    storeId,
                    order.Details,   // chi tiết cũ từ DB
                    newDetails,      // chi tiết mới vừa build
                    userId,
                    cancellationToken);

                // 8 Thêm notification
                await AddNotificationAsync(order, newStatus, userId, storeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng");
                return new UpdateOrderResponse(false, "Không thể cập nhật đơn hàng. Vui lòng thử lại.");
            }

            return new UpdateOrderResponse(true, "Cập nhật đơn hàng thành công");
        }

        #region Helpers

        public async Task ReserveStockAsync(
              Guid storeId,
              IEnumerable<OrderDetailDto> oldDetails,
              IEnumerable<OrderDetail> newDetails,
              Guid userId,
              CancellationToken cancellationToken)
        {
            var productIds = oldDetails.Select(d => d.ProductId)
                .Union(newDetails.Select(d => d.ProductId))
                .Distinct()
                .ToList();

            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var newDetail in newDetails)
            {
                var productId = newDetail.ProductId;

                if (!stockDict.TryGetValue(productId, out var stock))
                    throw new InvalidOperationException($"Không tìm thấy tồn kho cho sản phẩm {productId}");

                var oldQty = oldDetails.FirstOrDefault(d => d.ProductId == productId)?.Quantity ?? 0;
                var diff = newDetail.Quantity - oldQty; // số lượng thay đổi

                // Nếu cần thêm hàng mà không đủ Available
                if (diff > 0 && stock.Available < diff)
                {
                    throw new InvalidOperationException(
                        $"Sản phẩm {productId} không đủ tồn. Khả dụng {stock.Available}, cần thêm {diff}");
                }

                // Cập nhật lại Reserved = Reserved cũ - oldQty + newQty
                stock.Reserved = stock.Reserved - oldQty + newDetail.Quantity;
                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            // Nếu có sản phẩm bị remove trong newDetails thì trả lại reserved
            foreach (var oldDetail in oldDetails)
            {
                if (!newDetails.Any(d => d.ProductId == oldDetail.ProductId))
                {
                    if (stockDict.TryGetValue(oldDetail.ProductId, out var stock))
                    {
                        stock.Reserved -= oldDetail.Quantity;
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
                }
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }


        private async Task<(bool Success, string? ErrorMessage)> HandleStatusTransitionAsync(
            _Order order,
            _Status currentStatus,
            _Status newStatus,
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            if (currentStatus.Id == newStatus.Id) return (true, null);

            var productIds = order.Details.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            if (newStatus.Code == "COMPLETED")
            {
                foreach (var d in order.Details)
                {
                    if (!stockDict.TryGetValue(d.ProductId, out var stock))
                        return (false, $"Không tìm thấy tồn kho cho {d.ProductName}");
                    if (stock.Quantity < d.Quantity)
                        return (false, $"Không đủ tồn để hoàn thành {d.ProductName}");

                    stock.Quantity -= d.Quantity;
                    stock.Reserved = Math.Max(0, stock.Reserved - d.Quantity);
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;
                }
            }
            else if (newStatus.Code == "CANCELLED")
            {
                foreach (var d in order.Details)
                {
                    if (stockDict.TryGetValue(d.ProductId, out var stock))
                    {
                        stock.Reserved = Math.Max(0, stock.Reserved - d.Quantity);
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
                }
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
            return (true, null);
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
            order.StatusId = request.StatusId;
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
        #endregion
    }
}
