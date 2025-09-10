using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.Order.Queries.GetOrder
{
    public class GetOrderHandler : IRequestHandler<GetOrderRequest, GetOrderResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrderHandler> _logger;

        #region constructor
        public GetOrderHandler(
            IOrderRepository OrderRepository,
            IMapper mapper,
            ILogger<GetOrderHandler> logger)
        {
            _OrderRepository = OrderRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        public async Task<GetOrderResponse> Handle(GetOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var Order = await _OrderRepository.GetByIdAsync(request.Id);

                var mappedOrder = _mapper.Map<OrderCoreDto>(Order);
                return new GetOrderResponse(mappedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn đơn hàng.");
                return new GetOrderResponse(false, "Không thể tải đơn hàng. Vui lòng thử lại.");
            }
        }
    }
}
