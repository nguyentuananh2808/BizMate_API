using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevel
{
    public class GetDealerLevelHandler : IRequestHandler<GetDealerLevelRequest, GetDealerLevelResponse>
    {
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDealerLevelHandler> _logger;

        #region constructor
        public GetDealerLevelHandler(
            IDealerLevelRepository DealerLevelRepository,
            IMapper mapper,
            ILogger<GetDealerLevelHandler> logger)
        {
            _DealerLevelRepository = DealerLevelRepository;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion
        public async Task<GetDealerLevelResponse> Handle(GetDealerLevelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var DealerLevel = await _DealerLevelRepository.GetByIdAsync(request.Id, cancellationToken);

                var mappedDealerLevel = _mapper.Map<DealerLevelCoreDto>(DealerLevel);
                return new GetDealerLevelResponse(mappedDealerLevel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn bảng giá theo đại lý.");
                return new GetDealerLevelResponse(false, "Không thể tải bảng giá theo đại lý. Vui lòng thử lại.");
            }
        }
    }
}
