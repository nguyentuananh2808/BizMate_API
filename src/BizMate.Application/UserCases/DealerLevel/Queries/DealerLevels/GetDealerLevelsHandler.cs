using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels
{
    public class GetDealerLevelsHandler : IRequestHandler<GetDealerLevelsRequest, GetDealerLevelsResponse>
    {
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _queryFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDealerLevelsHandler> _logger;

        #region constructor
        public GetDealerLevelsHandler(
            IDealerLevelRepository DealerLevelRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetDealerLevelsHandler> logger)
        {
            _DealerLevelRepository = DealerLevelRepository;
            _userSession = userSession;
            _queryFactory = queryFactory;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetDealerLevelsResponse> Handle(GetDealerLevelsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var (DealerLevels, totalCount) = await _DealerLevelRepository.SearchDealerLevelsWithPaging(
                    storeId: storeId,
                    keyword: request.KeySearch,
                    pageIndex: request.PageIndex,
                    pageSize: request.PageSize,
                    queryFactory: _queryFactory);

                var mappedDealerLevels = _mapper.Map<List<DealerLevelCoreDto>>(DealerLevels);
                return new GetDealerLevelsResponse(mappedDealerLevels, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách bảng giá theo đại lý.");
                return new GetDealerLevelsResponse(false, "Không thể tải danh sách bảng giá theo đại lý. Vui lòng thử lại.");
            }
        }
    }
}