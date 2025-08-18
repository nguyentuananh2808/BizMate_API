using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel
{
    public class UpdateDealerLevelHandler : IRequestHandler<UpdateDealerLevelRequest, UpdateDealerLevelResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateDealerLevelHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateDealerLevelHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IDealerLevelRepository DealerLevelRepository,
            QueryFactory db,
            ILogger<UpdateDealerLevelHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _DealerLevelRepository = DealerLevelRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion
        public async Task<UpdateDealerLevelResponse> Handle(UpdateDealerLevelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                #region check DealerLevel exist
                var DealerLevel = await _DealerLevelRepository.GetByIdAsync(request.Id);
                if (DealerLevel == null)
                {
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateDealerLevelResponse(false, "Bảng giá theo đại lý không tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (DealerLevel.RowVersion != request.RowVersion)
                    return new UpdateDealerLevelResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
                #endregion

                #region update data
                DealerLevel.Name = request.Name;
                DealerLevel.StoreId = storeId;
                DealerLevel.UpdatedBy = Guid.Parse(userId);
                DealerLevel.UpdatedDate = DateTime.UtcNow;
                DealerLevel.IsActive = request.IsActive;
                DealerLevel.RowVersion = Guid.NewGuid();

                await _DealerLevelRepository.UpdateAsync(DealerLevel);
                #endregion

                var DealerLevelDto = _mapper.Map<DealerLevelCoreDto>(DealerLevel);

                return new UpdateDealerLevelResponse(DealerLevelDto, true, "Cập nhật bảng giá theo đại lý thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật bảng giá theo đại lý.");
                return new UpdateDealerLevelResponse(false, "Không thể cập nhật bảng giá theo đại lý. Vui lòng thử lại.");
            }
        }
    }
}
