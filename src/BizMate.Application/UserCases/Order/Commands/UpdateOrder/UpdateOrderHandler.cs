using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Domain.Entities.Order;
using _Status = BizMate.Domain.Entities.Status;
using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IStatusRepository _statusRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IOrderDetailRepository _detailRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UpdateOrderHandler> _logger;

        public UpdateOrderHandler(
            IStatusRepository statusRepository,
            INotificationRepository notificationRepository,
            IOrderDetailRepository detailRepository,
            IProductRepository productRepository,
            IAppMessageService messageService,
            IUserSession userSession,
            IOrderRepository orderRepository,
            ILogger<UpdateOrderHandler> logger)
        {
            _statusRepository = statusRepository;
            _notificationRepository = notificationRepository;
            _detailRepository = detailRepository;
            _messageService = messageService;
            _productRepository = productRepository;
            _userSession = userSession;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<UpdateOrderResponse> Handle(UpdateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _userSession.UserId;
                var storeId = _userSession.StoreId;

                var order = await _orderRepository.GetByIdAsync(request.Id);
                var details = request.Details;
                if (order == null)
                    return NotFoundResponse(request.Id);

                var status = await ValidateStatusAsync(request.StatusId, cancellationToken);
                if (status == null)
                    return new UpdateOrderResponse(false, "Không thể cập nhật đơn hàng. Trạng thái không hợp lệ.");

                if (!ValidateRowVersion(order, request.RowVersion))
                    return new UpdateOrderResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");

                UpdateOrderInfo(order, request, userId);

                await UpdateOrderDetailsAsync(order.Id, details);

                await _orderRepository.UpdateAsync(order);

                await AddNotificationAsync(order, status, Guid.Parse(userId), storeId);

                return new UpdateOrderResponse(true, "Cập nhật đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật đơn hàng.");
                return new UpdateOrderResponse(false, "Không thể cập nhật đơn hàng. Vui lòng thử lại.");
            }
        }

        #region Helpers
        private UpdateOrderResponse NotFoundResponse(Guid id)
        {
            var message = _messageService.NotExist(id.ToString());
            _logger.LogWarning(message);
            return new UpdateOrderResponse(false, "Đơn hàng không tồn tại.");
        }

        private async Task<_Status?> ValidateStatusAsync(Guid statusId, CancellationToken cancellationToken)
        {
            var status = await _statusRepository.GetIdById(statusId, cancellationToken);
            if (status == null)
            {
                _logger.LogWarning("Không tìm thấy trạng thái cho đơn hàng.");
                return null;
            }

            if (status.Code is "COMPLETED" or "CANCELLED")
            {
                _logger.LogWarning("Trạng thái hiện tại của đơn hàng không được phép cập nhật.");
                return null;
            }

            return status;
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
        }

        private async Task UpdateOrderDetailsAsync(Guid orderId, IEnumerable<UpdateOrderDetailRequest> details)
        {
            var existingDetails = await _detailRepository.GetByOrderIdAsync(orderId);
            var requestProductIds = details.Select(d => d.ProductId).ToList();

            // Lấy toàn bộ sản phẩm cần thiết 1 lần
            var products = await _productRepository.GetByIdsAsync(requestProductIds);
            var productDict = products.ToDictionary(p => p.Id, p => p);

            foreach (var item in details)
            {
                var existing = existingDetails.FirstOrDefault(d => d.ProductId == item.ProductId);
                if (existing != null)
                {
                    existing.Quantity = item.Quantity;
                    await _detailRepository.UpdateAsync(existing);
                }
                else if (productDict.TryGetValue(item.ProductId, out var product))
                {
                    await _detailRepository.AddAsync(new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        OrderId = orderId,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        Unit = product.Unit
                    });
                }
            }

            var toDelete = existingDetails.Where(d => !requestProductIds.Contains(d.ProductId)).ToList();
            if (toDelete.Any())
                await _detailRepository.DeleteRangeAsync(toDelete.Select(x => x.Id).ToList());
        }

        private async Task AddNotificationAsync(_Order order, _Status status, Guid userId, Guid storeId)
        {
            var notif = new NotificationDto
            {
                UserId = null,//gửi tất cả user trong cửa hàng
                OrderId = order.Id,
                StoreId = storeId,
                Message = $"Đơn hàng #{order.Code} đã được cập nhật thành công",
                Type = "order"
            };

            await _notificationRepository.BroadcastAsync(notif);
        }
        #endregion
    }
}
