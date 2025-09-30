using BizMate.Application.Common.Dto.CoreDto;
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
        private readonly IOrderRepository _orderRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateStatusOrderHandler> _logger;

        #region constructor
        public UpdateStatusOrderHandler(
            IOrderStatusHistoryRepository orderStatusHistoryRepository,
            IMediator mediator,
            IStockRepository stockRepository,
            IStatusRepository statusRepository,
            IUserSession userSession,
            IOrderRepository OrderRepository,
            ILogger<UpdateStatusOrderHandler> logger)
        {
            _orderStatusHistoryRepository = orderStatusHistoryRepository;
            _mediator = mediator;
            _stockRepository = stockRepository;
            _statusRepository = statusRepository;
            _userSession = userSession;
            _orderRepository = OrderRepository;
            _logger = logger;
        }
        #endregion

        public async Task<UpdateStatusOrderResponse> Handle(UpdateStatusOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = Guid.Parse(_userSession.UserId);

                #region check Order exist
                var order = await _orderRepository.GetByIdAsync(request.Id);
                if (order == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }
                #endregion

                #region check rowversion
                if (order.RowVersion != request.RowVersion)
                {
                    var message = ValidationMessage.LocalizedStrings.NotValidRowversion;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }
                #endregion

                #region get status
                //lấy id trạng thái muốn cập nhật
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(request.StatusCode, "Order");
                if (statusId == Guid.Empty)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist2;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }
                ///lấy id trạng thái hiện tại
                var currentStatus = await _statusRepository.GetIdById(request.StatusId, cancellationToken);
                if (currentStatus == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist2;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }
                ///lấy status trạng thái muốn cập nhật
                var updateStatus = await _statusRepository.GetIdById(statusId, cancellationToken);
                if (currentStatus == null)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist2;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }
                #endregion

                #region logic check 
                //nếu là trạng thái hiện tại là "Hủy" hoặc "Hoàn thành" thì không được đổi trạng thái
                if (currentStatus.Code == "CANCELLED" || currentStatus.Code == "COMPLETED")
                {
                    var message = ValidationMessage.LocalizedStrings.RoleWithoutAuthority;
                    _logger.LogWarning(message);
                    return new UpdateStatusOrderResponse(false, message);
                }

                //nếu trạng thái cập nhật là Hủy thì trả lại số lượng đã giữ chỗ về tồn kho
                if (updateStatus?.Code == "CANCELLED")
                {
                    await ReleaseReservedStockAsync(storeId, order.Details.Select(d => new OrderDetail
                    {
                        ProductId = d.ProductId,
                        Quantity = d.Quantity
                    }), userId, cancellationToken);
                }

                //nếu trạng thái hiện tại là "Đã đóng hàng" cập nhật thành "Hoàn thành"
                //thì thực hiện trừ tồn kho và tạo tự động phiếu xuất kho
                if (currentStatus.Code == "PACKED" && updateStatus?.Code == "COMPLETED")
                {
                    await CreateExportReceipt(order, cancellationToken);
                    await DeductStockAsync(storeId, order.Details, userId, cancellationToken);
                }

                #endregion

                #region update status
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
                #endregion
                #region update status & log history
                await _orderStatusHistoryRepository.UpdateOrderStatus(
                    request.Id,
                    statusId,
                    userId,
                    _userSession.UserName ?? "Unknown",
                    "Đổi trạng thái từ " + currentStatus.Name + " thành " + updateStatus?.Name
                );
                #endregion


                return new UpdateStatusOrderResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái phiếu nhập kho.");
                return new UpdateStatusOrderResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }

        /// <summary>
        /// Cập nhật tồn kho theo phiếu xuất
        /// </summary>
        public async Task DeductStockAsync(Guid storeId, IEnumerable<OrderDetailDto> orderDetails, Guid userId, CancellationToken cancellationToken)
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

        //order ở trạng thái hủy thì sẽ trả lại số lượng đã giữ chỗ về tồn kho
        public async Task ReleaseReservedStockAsync(Guid storeId, IEnumerable<OrderDetail> orderDetails, Guid userId, CancellationToken cancellationToken)
        {
            var productIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var detail in orderDetails)
            {
                if (!stockDict.TryGetValue(detail.ProductId, out var stock))
                    continue;

                stock.Reserved -= detail.Quantity;
                if (stock.Reserved < 0) stock.Reserved = 0; // tránh âm

                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }



    }
}
