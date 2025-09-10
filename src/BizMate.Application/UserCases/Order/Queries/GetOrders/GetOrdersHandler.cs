using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Order.Queries.GetOrders
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersRequest, GetOrdersResponse>
    {
        private readonly IOrderRepository _OrderRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetOrdersHandler> _logger;

        public GetOrdersHandler(
            IOrderRepository OrderRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetOrdersHandler> logger)
        {
            _OrderRepository = OrderRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<GetOrdersResponse> Handle(GetOrdersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (receipts, totalCount) = await _OrderRepository.SearchReceiptsWithPaging(
                    storeId: storeId,
                    dateFrom: request.DateFrom,
                    dateTo: request.DateTo,
                    statusIds: request.StatusIds,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory
                );

                var mapped = _mapper.Map<List<OrderCoreDto>>(receipts);

                return new GetOrdersResponse(mapped, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách phiếu nhập.");
                return new GetOrdersResponse(false, "Không thể tải danh sách phiếu nhập kho. Vui lòng thử lại.");
            }
        }
    }
}
