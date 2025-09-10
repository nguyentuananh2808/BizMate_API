using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Domain.Entities.Order;
using BizMate.Public.Message;

namespace BizMate.Application.UserCases.Order.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderRequest, CreateOrderResponse>
    {
        private readonly IStockRepository _stockRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IUserSession _userSession;
        private readonly IProductRepository _productRepository;
        private readonly IStatusRepository _statusRepository;
        private readonly ILogger<CreateOrderHandler> _logger;

        #region constructor
        public CreateOrderHandler(
            IStockRepository stockRepository,   
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IOrderRepository OrderRepository,
            ILogger<CreateOrderHandler> logger)
        {
            _stockRepository = stockRepository;
            _productRepository = productRepository;
            _statusRepository = statusRepository;
            _codeGeneratorService = codeGeneratorService;
            _userSession = userSession;
            _orderRepository = OrderRepository;
            _logger = logger;
        }
        #endregion
        public async Task<CreateOrderResponse> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;


                #region create new Order
                var productIds = request.Details.Select(d => d.ProductId).ToList();

                var products = await _productRepository.GetByIdsAsync(productIds);

                var productDict = products.ToDictionary(p => p.Id);

                //get status for Order
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync("NEW", "Order");
                if (statusId == Guid.Empty)
                {
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new CreateOrderResponse(false, message);
                }
                var receiptCode = await _codeGeneratorService.GenerateCodeAsync("#NK");

                var receiptId = Guid.NewGuid();
                var newOrder = new _Order
                {
                    Id = receiptId,
                    Code = receiptCode,
                    StoreId = storeId,
                    CreatedBy = Guid.Parse(userId),
                    CustomerId = request.CustomerId,
                    CustomerName = request.CustomerName,
                    CustomerPhone = request.CustomerPhone,
                    CustomerType = request.CustomerType,
                    DeliveryAddress = request.DeliveryAddress,
                    StatusId = statusId,
                    Description = request.Description,
                    Details = request.Details.Select(d =>
                    {
                        var product = productDict[d.ProductId];

                        return new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = receiptId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            UnitPrice = d.UnitPrice,
                            Unit = product.Unit,
                        };
                    }).ToList()
                };

                await _orderRepository.AddAsync(newOrder, cancellationToken);

                //tăng số lượng sản phẩm giữ chỗ
                await ReserveStockAsync(storeId, newOrder.Details, Guid.Parse(userId), cancellationToken);
                #endregion

                return new CreateOrderResponse(true, "Tạo đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo đơn hàng.");
                return new CreateOrderResponse(false, "Không thể tạo đơn hàng. Vui lòng thử lại.");
            }
        }

        public async Task ReserveStockAsync(Guid storeId, IEnumerable<OrderDetail> orderDetails, Guid userId, CancellationToken cancellationToken)
        {
            var productIds = orderDetails.Select(d => d.ProductId).Distinct().ToList();
            var stocks = await _stockRepository.GetByStoreAndProductAsync(storeId, productIds);
            var stockDict = stocks.ToDictionary(s => s.ProductId);

            foreach (var detail in orderDetails)
            {
                if (!stockDict.TryGetValue(detail.ProductId, out var stock))
                    throw new InvalidOperationException($"Không tìm thấy tồn kho cho sản phẩm {detail.ProductId}");

                if (stock.Available < detail.Quantity)
                    throw new InvalidOperationException($"Sản phẩm {detail.ProductId} không đủ tồn. Khả dụng {stock.Available}, cần {detail.Quantity}");

                stock.Reserved += detail.Quantity;
                stock.UpdatedBy = userId;
                stock.UpdatedDate = DateTime.UtcNow;
            }

            await _stockRepository.UpdateAsync(stockDict.Values, cancellationToken);
        }

    }
}
