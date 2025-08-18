using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Customer.Queries.Customers
{
    public class GetCustomersHandler : IRequestHandler<GetCustomersRequest, GetCustomersResponse>
    {
        private readonly ICustomerRepository _CustomerRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetCustomersHandler> _logger;

        #region constructor
        public GetCustomersHandler(
            ICustomerRepository CustomerRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetCustomersHandler> logger)
        {
            _CustomerRepository = CustomerRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetCustomersResponse> Handle(GetCustomersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (Customers, totalCount) = await _CustomerRepository.SearchCustomersWithPaging(
                    storeId: storeId,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    isActive: request.IsActive,
                    queryFactory: _queryFactory);

                var mappedCustomers = _mapper.Map<List<CustomerCoreDto>>(Customers);
                return new GetCustomersResponse(mappedCustomers, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách khách hàng.");
                return new GetCustomersResponse(false, "Không thể tải danh sách khách hàng. Vui lòng thử lại.");
            }
        }
    }
}
