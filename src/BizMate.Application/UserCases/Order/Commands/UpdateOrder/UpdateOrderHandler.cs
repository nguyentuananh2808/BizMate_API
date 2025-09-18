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

            // 1. Lấy order hiện tại (kèm details)
            var order = await _orderRepository.GetByIdAsync(request.Id);
            if (order == null) return NotFoundResponse(request.Id);

            // 2. Lấy status hiện tại và status đích (nếu thay đổi)
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

            // 3. Validate không cho phép cập nhật nếu đơn đã Hoàn thành hoặc Hủy
            if (currentStatus.Code == "COMPLETED" || currentStatus.Code == "CANCELLED")
            {
                _logger.LogWarning("Đơn hàng đã ở trạng thái không cho phép cập nhật.");
                return new UpdateOrderResponse(false, "Đơn hàng đã hoàn thành hoặc hủy, không thể cập nhật");
            }

            // 4. Validate RowVersion (bạn đã yêu cầu không thay đổi phần rowversion logic)
            if (!ValidateRowVersion(order, request.RowVersion))
                return new UpdateOrderResponse(false, "Dữ liệu đã thay đổi, vui lòng tải lại");

            // 5. Bắt đầu transaction - tất cả thay đổi sẽ được commit 1 lần
            try
            {
                // 1. Bắt đầu transaction thông qua UnitOfWork
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // 2. Cập nhật thông tin đơn hàng cơ bản (không set Status navigation để EF không eager load conflict)
                UpdateOrderInfo(order, request, _userSession.UserId);

                // 3. Cập nhật chi tiết đơn và stock (chỉ chỉnh in-memory, chưa save)
                var detailResult = await UpdateOrderDetailsAsync(order, request.Details, userId, storeId, cancellationToken);
                if (!detailResult.Success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new UpdateOrderResponse(false, detailResult.ErrorMessage);
                }

                // 4. Nếu có chuyển trạng thái -> xử lý chuyển trạng thái liên quan tới stock (ví dụ COMPLETED, CANCELLED)
                var stockTransitionResult = await HandleStatusTransitionAsync(order, currentStatus, newStatus, userId, storeId, cancellationToken);
                if (!stockTransitionResult.Success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return new UpdateOrderResponse(false, stockTransitionResult.ErrorMessage);
                }

                // 5. Recalculate order total
                order.RecalculateTotal();

                // 6. Commit tất cả thay đổi xuống DB 1 lần duy nhất
                await _unitOfWork.CommitAsync(cancellationToken);

                // 7. Sau khi commit xong, gửi thông báo (push, broadcast...) - notification repo có thể xử lý async
                await AddNotificationAsync(order, newStatus, userId, storeId);
            }
            catch (Exception ex)
            {
                // 8. Rollback transaction nếu có lỗi
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error updating order");
                throw;
            }


            return new UpdateOrderResponse(true, "Cập nhật đơn hàng thành công");
        }

        /// <summary>
        /// Cập nhật chi tiết đơn: thêm, sửa, xóa — đồng thời tính diff để điều chỉnh Reserved tạm thời.
        /// Không gọi SaveChanges; chỉ thay đổi tracked entities để save 1 lần ở handler.
        /// Trả về failure nếu yêu cầu tăng Reserved vượt quá Quantity (không đủ hàng).
        /// </summary>
        private async Task<(bool Success, string? ErrorMessage)> UpdateOrderDetailsAsync(
            _Order order,
            IEnumerable<UpdateOrderDetailRequest> details,
            Guid userId,
            Guid storeId,
            CancellationToken cancellationToken)
        {
            var existingDetails = order.Details.ToList(); // tracked từ order
            var requestProductIds = details.Select(d => d.ProductId).Distinct().ToList();

            // Lấy toàn bộ sản phẩm cần 1 lần
            var products = await _productRepository.GetByIdsAsync(requestProductIds);
            var productDict = products.ToDictionary(p => p.Id, p => p);

            // Lấy tồn kho cho store & các product
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, requestProductIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            // 1. xử lý thêm / cập nhật
            foreach (var item in details)
            {
                var existing = existingDetails.FirstOrDefault(d => d.ProductId == item.ProductId);

                if (existing != null)
                {
                    var diff = item.Quantity - existing.Quantity; // positive => cần tăng reserved, negative => trả reserved
                    // kiểm tra đủ hàng nếu tăng reserved
                    if (diff > 0)
                    {
                        if (!stockDict.TryGetValue(item.ProductId, out var stock))
                        {
                            return (false, $"Không tìm thấy tồn kho cho sản phẩm {item.ProductId}");
                        }
                        if (stock.Available < diff)
                        {
                            return (false, $"Sản phẩm {existing.ProductName} không đủ tồn kho. Yêu cầu tăng {diff}, khả dụng {stock.Available}");
                        }

                        stock.Reserved += diff;
                        stock.UpdatedBy = userId;
                        stock.UpdatedDate = DateTime.UtcNow;
                    }
                    else if (diff < 0)
                    {
                        if (stockDict.TryGetValue(item.ProductId, out var stock))
                        {
                            stock.Reserved += diff; // diff negative -> giảm reserved
                            if (stock.Reserved < 0) stock.Reserved = 0;
                            stock.UpdatedBy = userId;
                            stock.UpdatedDate = DateTime.UtcNow;
                        }
                    }

                    existing.Quantity = item.Quantity;
                    existing.UnitPrice = existing.UnitPrice == 0 ? (productDict.TryGetValue(item.ProductId, out var pd) ? pd.SalePrice.GetValueOrDefault(0) : 0) : existing.UnitPrice;
                    existing.Total = existing.Quantity * existing.UnitPrice;
                    existing.UpdatedBy = userId;
                    existing.UpdatedDate = DateTime.UtcNow;

                    // Update tracked entity (EF is tracking order.Details; nếu repository method cần gọi, gọi sau)
                    _detailRepository.UpdateAsync(existing, cancellationToken);
                }
                else
                {
                    if (!productDict.TryGetValue(item.ProductId, out var product))
                    {
                        return (false, $"Sản phẩm {item.ProductId} không tồn tại");
                    }

                    // kiểm tra tồn kho trước khi reserve
                    if (!stockDict.TryGetValue(item.ProductId, out var stockForAdd))
                    {
                        return (false, $"Không tìm thấy tồn kho cho sản phẩm {product.Name}");
                    }
                    if (stockForAdd.Available < item.Quantity)
                    {
                        return (false, $"Sản phẩm {product.Name} không đủ tồn kho. Yêu cầu {item.Quantity}, khả dụng {stockForAdd.Available}");
                    }

                    var newDetail = new OrderDetail
                    {
                        Id = Guid.NewGuid(),
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        OrderId = order.Id,
                        ProductName = product.Name,
                        ProductCode = product.Code,
                        Unit = product.Unit,
                        UnitPrice = product.SalePrice.GetValueOrDefault(0),
                        Total = item.Quantity * product.SalePrice.GetValueOrDefault(0),
                        CreatedBy = userId,
                        CreatedDate = DateTime.UtcNow
                    };

                    order.Details.Add(newDetail);
                    _detailRepository.AddAsync(newDetail, cancellationToken);

                    // adjust reserved
                    stockForAdd.Reserved += item.Quantity;
                    stockForAdd.UpdatedBy = userId;
                    stockForAdd.UpdatedDate = DateTime.UtcNow;
                }
            }

            // 2. xử lý xóa sản phẩm (những existing không còn trong request)
            var toDelete = existingDetails.Where(d => !requestProductIds.Contains(d.ProductId)).ToList();
            foreach (var del in toDelete)
            {
                if (stockDict.TryGetValue(del.ProductId, out var stock))
                {
                    stock.Reserved -= del.Quantity;
                    if (stock.Reserved < 0) stock.Reserved = 0;
                    stock.UpdatedBy = userId;
                    stock.UpdatedDate = DateTime.UtcNow;
                }

                order.Details.Remove(del);
                _detailRepository.DeleteAsync(del.Id, cancellationToken);
            }

            // 3. Update stocks (luôn sửa tracked stock entities trong repository)
            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);

            return (true, null);
        }

        /// <summary>
        /// Xử lý logic khi thay đổi trạng thái giữa currentStatus -> newStatus
        /// Ví dụ:
        /// - Nếu chuyển sang COMPLETED: trừ Quantity (tồn thực tế) tương ứng và giảm Reserved.
        /// - Nếu chuyển sang CANCELLED: trả Reserved về (Reserved -= qty)
        /// - Các transition khác không tác động Quantity (chỉ có thể giữ reserved đã được xử lý phía trên)
        /// </summary>
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
                // trừ chính thức Quantity và giảm Reserved
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

                // có thể thêm logic tạo ExportReceipt / giao vận ở đây
            }
            else if (newStatus.Code == "CANCELLED")
            {
                // trả reserved về
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
            // Các trạng thái khác (PACKING, PACKED, ...) không làm thay đổi Quantity, chỉ reserved được xử lý trong UpdateOrderDetailsAsync

            // Update stocks
            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);

            // Cập nhật status lên order (đã set trong UpdateOrderInfo từ request)
            // order.StatusId đã được set ở UpdateOrderInfo, chỉ cần cập nhật navigation nếu muốn
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
            order.RowVersion = Guid.NewGuid(); // bạn yêu cầu giữ rowversion logic, nên vẫn update
            order.StatusId = request.StatusId;
            order.Status = null;
        }

        private async Task AddNotificationAsync(_Order order, _Status status, Guid userId, Guid storeId)
        {
            var notif = new NotificationDto
            {
                UserId = null, // gửi tất cả user trong cửa hàng
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
