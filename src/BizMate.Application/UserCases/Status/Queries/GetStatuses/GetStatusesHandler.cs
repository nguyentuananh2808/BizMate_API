using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Status.Queries.GetStatuses
{
    public class GetStatusesHandler : IRequestHandler<GetStatusesRequest, GetStatusesResponse>
    {
        private readonly IStatusRepository _statuseRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<GetStatusesHandler> _logger;

        #region constructor
        public GetStatusesHandler(
            IStatusRepository StatuseRepository,
            IUserSession userSession,
            QueryFactory queryFactory,
            IMapper mapper,
            ILogger<GetStatusesHandler> logger)
        {
            _statuseRepository = StatuseRepository;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetStatusesResponse> Handle(GetStatusesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var statuses = await _statuseRepository.GetStatusesOfGroup(
                    group: request.Group);

                var mappedStatuses = _mapper.Map<List<StatuseCoreDto>>(statuses);
                return new GetStatusesResponse(mappedStatuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách trạng thái.");
                return new GetStatusesResponse(false, "Không thể tải danh sách trạng thái. Vui lòng thử lại.");
            }
        }
    }
}
