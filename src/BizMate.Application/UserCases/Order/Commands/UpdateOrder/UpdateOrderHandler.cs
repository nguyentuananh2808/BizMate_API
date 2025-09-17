using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Domain.Entities.Order;
using _Status = BizMate.Domain.Entities.Status;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderHandler : IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>
    {
        private readonly IStockRepository _stockRepository;
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
            IUserSession userSession,
            IOrderRepository orderRepository,
            ILogger<UpdateOrderHandler> logger,
            IStockRepository stockRepository)
        {
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

            // 1. Lấy order hiện tại
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) return NotFoundResponse(request.Id);

            // 2. Validate trạng thái
            var status = await ValidateStatusAsync(order.StatusId, cancellationToken);
            if (status == null) return new UpdateOrderResponse(false, "Trạng thái không hợp lệ");

            // 3. Check row version
            if (!ValidateRowVersion(order, request.RowVersion))
                return new UpdateOrderResponse(false, "Dữ liệu đã thay đổi, vui lòng tải lại");

            // 4. Cập nhật thông tin order
            UpdateOrderInfo(order, request, _userSession.UserId);

            await _orderRepository.UpdateAsync(order);

            // 5. Cập nhật chi tiết đơn hàng + reserved stock
            await UpdateOrderDetailsAsync(order.Id, request.Details, userId, storeId, cancellationToken);

            // 6. Bắn thông báo
            await AddNotificationAsync(order, status, userId, storeId);

            return new UpdateOrderResponse(true, "Cập nhật đơn hàng thành công");
        }

        private async Task UpdateOrderDetailsAsync(Guid orderId, IEnumerable<UpdateOrderDetailRequest> details, Guid userId, Guid storeId, CancellationToken cancellationToken)
        {
            var existingDetails = await _detailRepository.GetByOrderIdAsync(orderId);
            var requestProductIds = details.Select(d => d.ProductId).ToList();

            // Lấy toàn bộ sản phẩm cần thiết 1 lần
            var products = await _productRepository.GetByIdsAsync(requestProductIds);
            var productDict = products.ToDictionary(p => p.Id, p => p);

            // Lấy tồn kho
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, requestProductIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var item in details)
            {
                var existing = existingDetails.FirstOrDefault(d => d.ProductId == item.ProductId);

                if (existing != null)
                {
                    var diff = item.Quantity - existing.Quantity;
                    existing.Quantity = item.Quantity;
                    existing.Total = existing.Quantity * existing.UnitPrice;
                    await _detailRepository.UpdateAsync(existing);

                    if (diff != 0 && stockDict.TryGetValue(item.ProductId, out var stock))
                    {
                        stock.Reserved += diff;
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
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
                        Unit = product.Unit,
                        UnitPrice = product.SalePrice.GetValueOrDefault(0),
                        Total = item.Quantity * product.SalePrice.GetValueOrDefault(0)
                    });

                    if (stockDict.TryGetValue(item.ProductId, out var stock))
                    {
                        stock.Reserved += item.Quantity;
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
                }
            }

            // Xử lý xóa sản phẩm
            var toDelete = existingDetails.Where(d => !requestProductIds.Contains(d.ProductId)).ToList();
            foreach (var del in toDelete)
            {
                if (stockDict.TryGetValue(del.ProductId, out var stock))
                {
                    stock.Reserved -= del.Quantity;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;
                }
            }

            if (toDelete.Any())
                await _detailRepository.DeleteRangeAsync(toDelete.Select(x => x.Id).ToList());

            // Update lại tồn kho một lần
            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

        #region Helpers
        private UpdateOrderResponse NotFoundResponse(Guid id)
        {
            var message = ValidationMessage.LocalizedStrings.DataNotExist;
            _logger.LogWarning(message);
            return new UpdateOrderResponse(false, message);
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

        private async Task AddNotificationAsync(_Order order, _Status status, Guid userId, Guid storeId)
        {
            var notif = new NotificationDto
            {
                UserId = null, // gửi tất cả user trong cửa hàng
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
