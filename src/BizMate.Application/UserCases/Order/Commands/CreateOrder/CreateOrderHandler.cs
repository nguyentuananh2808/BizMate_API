using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using _Order = BizMate.Domain.Entities.Order;
using _DealerLevel = BizMate.Domain.Entities.DealerLevel;

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
        private readonly IStatusRepository _statusRepository;
        private readonly ILogger<CreateOrderHandler> _logger;

        #region constructor
        public CreateOrderHandler(
            IDealerLevelRepository dealerLevelRepository,
            ICustomerRepository customerRepository,
            IStockRepository stockRepository,
            IStatusRepository statusRepository,
            IProductRepository productRepository,
            ICodeGeneratorService codeGeneratorService,
            IUserSession userSession,
            IOrderRepository OrderRepository,
            ILogger<CreateOrderHandler> logger)
        {
            _dealerLevelRepository = dealerLevelRepository;
            _customerRepository = customerRepository;
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
                var receiptId = Guid.NewGuid();

                var statusCode = request.IsDraft ? "DRAFT" : "NEW";
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(statusCode, "Order");

                var code = await _codeGeneratorService.GenerateCodeAsync("#DH", 5);
                // Lấy customer
                var customer = request.CustomerId != null
                    ? await _customerRepository.GetByIdAsync(request.CustomerId.Value, cancellationToken)
                    : null;

                // Lấy product list cần thiết
                var productIds = request.Details.Select(d => d.ProductId).ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDict = products.ToDictionary(p => p.Id);

                // Nếu là đại lý thì lấy DealerLevel + DealerPrices
                _DealerLevel? dealerLevel = null;
                if (request.CustomerType == 2 && customer?.DealerLevelId != null)
                {
                    dealerLevel = await _dealerLevelRepository.GetByIdAsync(customer.DealerLevelId.Value, cancellationToken);
                }


                var newOrder = new _Order
                {
                    Id = receiptId,
                    Code = code,
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
                        decimal unitPrice;

                        if (request.CustomerType == 2 && dealerLevel != null)
                        {
                            // Tìm giá theo DealerLevel (đã load sẵn DealerPrices từ repository)
                            var dealerPrice = dealerLevel.DealerPrices
                                .FirstOrDefault(dp => dp.ProductId == d.ProductId && !dp.IsDeleted);

                            unitPrice = dealerPrice?.Price
                                        ?? product.SalePrice.GetValueOrDefault(0); // fallback nếu chưa có giá cho DealerLevel
                        }
                        else
                        {
                            // Khách thường hoặc đại lý chưa có DealerLevel
                            unitPrice = product.SalePrice.GetValueOrDefault(0);
                        }

                        return new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            OrderId = receiptId,
                            ProductId = d.ProductId,
                            Quantity = d.Quantity,
                            ProductName = product.Name,
                            ProductCode = product.Code,
                            UnitPrice = unitPrice,
                            Unit = product.Unit,
                            Total = d.Quantity * unitPrice
                        };
                    }).ToList()
                };

                newOrder.RecalculateTotal();

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
