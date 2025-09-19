using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Domain.Entities.Order;
using _Status = BizMate.Domain.Entities.Status;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Public.Message;
using Microsoft.EntityFrameworkCore;
using BizMate.Application.Common.Interfaces;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationRepository _notificationRepository;
        private readonly IOrderDetailRepository _detailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateOrderHandler> _logger;
        private readonly IStockRepository _stockRepository;

        public UpdateOrderHandler(
            IUnitOfWork unitOfWork,
            IStatusRepository statusRepository,
            INotificationRepository notificationRepository,
            IOrderDetailRepository detailRepository,
            IProductRepository productRepository,
            IUserSession userSession,
            IOrderRepository orderRepository,
            ILogger<UpdateOrderHandler> logger,
            IStockRepository stockRepository)
        {
            _unitOfWork = unitOfWork;
            _statusRepository = statusRepository;
            _notificationRepository = notificationRepository;
            _detailRepository = detailRepository;
            _productRepository = productRepository;
            _userSession = userSession;
            _orderRepository = orderRepository;
            _logger = logger;
            _stockRepository = stockRepository;
        }

        public async Task<UpdateOrderResponse> Handle(UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            var userId = Guid.Parse(_userSession.UserId);
            var storeId = _userSession.StoreId;

            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) return NotFoundResponse(request.Id);

            var currentStatus = await _statusRepository.GetIdById(order.StatusId, cancellationToken);
            if (currentStatus == null)
            {
                _logger.LogWarning("Trạng thái hiện tại của đơn hàng không tồn tại.");
                return new UpdateOrderResponse(false, "Trạng thái hiện tại không hợp lệ");
            }

            var newStatus = await _statusRepository.GetIdById(request.StatusId, cancellationToken);
            if (newStatus == null)
            {
                _logger.LogWarning("Trạng thái mới không tồn tại.");
                return new UpdateOrderResponse(false, "Trạng thái mới không hợp lệ");
            }

            if (currentStatus.Code == "COMPLETED" || currentStatus.Code == "CANCELLED")
            {
                _logger.LogWarning("Đơn hàng đã ở trạng thái không cho phép cập nhật.");
                return new UpdateOrderResponse(false, "Đơn hàng đã hoàn thành hoặc hủy, không thể cập nhật");
            }

            if (!ValidateRowVersion(order, request.RowVersion))
                return new UpdateOrderResponse(false, "Dữ liệu đã thay đổi, vui lòng tải lại");

            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                UpdateOrderInfo(order, request, _userSession.UserId);

                // --- Cập nhật chi tiết đơn và stock ---
                var detailResult = await UpdateOrderDetailsAsync(order, request.Details, userId, storeId, cancellationToken);
                if (!detailResult.Success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new UpdateOrderResponse(false, detailResult.ErrorMessage);
                }

                var stockTransitionResult = await HandleStatusTransitionAsync(order, currentStatus, newStatus, userId, storeId, cancellationToken);
                if (!stockTransitionResult.Success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new UpdateOrderResponse(false, stockTransitionResult.ErrorMessage);
                }

                order.RecalculateTotal();

                await _unitOfWork.CommitAsync(cancellationToken);

                await AddNotificationAsync(order, newStatus, userId, storeId);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error updating order");
                throw;
            }

            return new UpdateOrderResponse(true, "Cập nhật đơn hàng thành công");
        }

        /// <summary>
        /// Cập nhật chi tiết đơn: xóa chi tiết cũ bằng repository và thêm chi tiết mới
        /// </summary>
        private async Task<(bool Success, string? ErrorMessage)> UpdateOrderDetailsAsync(
            _Order order,
            IEnumerable<UpdateOrderDetailRequest> details,
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var requestProductIds = details.Select(d => d.ProductId).Distinct().ToList();

            var products = await _productRepository.GetByIdsAsync(requestProductIds);
            var productDict = products.ToDictionary(p => p.Id, p => p);

            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, requestProductIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            // --- Xóa chi tiết cũ bằng repository, không xóa collection trực tiếp ---
            foreach (var oldDetail in order.Details.ToList())
            {
                if (stockDict.TryGetValue(oldDetail.ProductId, out var stock))
                {
                    stock.Reserved -= oldDetail.Quantity;
                    if (stock.Reserved < 0) stock.Reserved = 0;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;
                }

                await _detailRepository.DeleteAsync(oldDetail.Id, cancellationToken);
                //order.Details.Remove(oldDetail);
            }

            // --- Thêm chi tiết mới ---
            foreach (var item in details)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    return (false, $"Sản phẩm {item.ProductId} không tồn tại");

                if (!stockDict.TryGetValue(item.ProductId, out var stock))
                    return (false, $"Không tìm thấy tồn kho cho sản phẩm {product.Name}");

                if (stock.Available < item.Quantity)
                    return (false, $"Sản phẩm {product.Name} không đủ tồn kho. Yêu cầu {item.Quantity}, khả dụng {stock.Available}");

                var newDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    ProductName = product.Name,
                    ProductCode = product.Code,
                    Unit = product.Unit,
                    Quantity = item.Quantity,
                    UnitPrice = product.SalePrice.GetValueOrDefault(0),
                    Total = item.Quantity * product.SalePrice.GetValueOrDefault(0),
                    CreatedBy = userId,
                    CreatedDate = DateTime.UtcNow
                };

                order.Details.Add(newDetail);
                await _detailRepository.AddAsync(newDetail, cancellationToken);

                stock.Reserved += item.Quantity;
                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            // --- Cập nhật stock 1 lần ---
            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);

            return (true, null);
        }

        private async Task<(bool Success, string? ErrorMessage)> HandleStatusTransitionAsync(
            _Order order,
            _Status currentStatus,
            _Status newStatus,
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            // Nếu trạng thái không đổi => nothing
            if (currentStatus.Id == newStatus.Id) return (true, null);

            // Lấy danh sách stock cho tất cả product trong order
            var productIds = order.Details.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            if (newStatus.Code == "COMPLETED")
            {
                foreach (var d in order.Details)
                {
                    if (!stockDict.TryGetValue(d.ProductId, out var stock))
                    {
                        return (false, $"Không tìm thấy tồn kho cho sản phẩm {d.ProductName} khi hoàn thành đơn");
                    }

                    if (stock.Quantity < d.Quantity)
                    {
                        return (false, $"Không đủ tồn thực tế để hoàn thành đơn cho sản phẩm {d.ProductName}. Yêu cầu {d.Quantity}, tồn {stock.Quantity}");
                    }

                    stock.Quantity -= d.Quantity;
                    stock.Reserved -= d.Quantity;
                    if (stock.Reserved < 0) stock.Reserved = 0;
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
                        stock.Reserved -= d.Quantity;
                        if (stock.Reserved < 0) stock.Reserved = 0;
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
                }
            }

            // Cập nhật stock 1 lần
            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);

            return (true, null);
        }

        #region Helpers
        private UpdateOrderResponse NotFoundResponse(Guid id)
        {
            var message = ValidationMessage.LocalizedStrings.DataNotExist;
            _logger.LogWarning(message);
            return new UpdateOrderResponse(false, message);
        }

        private static bool ValidateRowVersion(_Order order, Guid rowVersion) =>
            order.RowVersion == rowVersion;

        private static void UpdateOrderInfo(_Order order, UpdateOrderRequest request, string userId)
        {
            order.CustomerId = request.CustomerId;
            order.CustomerType = request.CustomerType;
            order.CustomerPhone = request.CustomerPhone;
            order.CustomerName = request.CustomerName;
            order.DeliveryAddress = request.DeliveryAddress;
            order.Description = request.Description;
            order.UpdatedDate = DateTime.UtcNow;
            order.UpdatedBy = Guid.Parse(userId);
            order.RowVersion = Guid.NewGuid();
            order.StatusId = request.StatusId;
            order.Status = null;
        }

        private async Task AddNotificationAsync(_Order order, _Status status, Guid userId, Guid storeId)
        {
            var notif = new NotificationDto
            {
                UserId = null,
                OrderId = order.Id,
                StoreId = storeId,
                Message = $"Đơn hàng #{order.Code} đã được cập nhật: {status.Name}",
                Type = "order"
            };

            await _notificationRepository.BroadcastAsync(notif);
        }
        #endregion
    }
}
