using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel
{
    public class DeleteDealerLevelHandler : IRequestHandler<DeleteDealerLevelRequest, DeleteDealerLevelResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteDealerLevelHandler> _logger;

        #region constructor
        public DeleteDealerLevelHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            IDealerLevelRepository DealerLevelRepository,
            ILogger<DeleteDealerLevelHandler> logger)
        {
            _messageService = messageService;
            _userSession = userSession;
            _DealerLevelRepository = DealerLevelRepository;
            _logger = logger;
        }
        #endregion
        public async Task<DeleteDealerLevelResponse> Handle(DeleteDealerLevelRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                #region check DealerLevel exist
                var DealerLevel = await _DealerLevelRepository.GetByIdAsync(request.Id);
                if (DealerLevel == null || DealerLevel.StoreId != storeId)
                {
                    var message = _messageService.NotExist(request.Id);
                    _logger.LogWarning(message);
                    return new DeleteDealerLevelResponse(false, "Bảng giá theo đại lý không tồn tại.");
                }
                #endregion

                #region check if DealerLevel is used

                #endregion
                await _DealerLevelRepository.DeleteAsync(request.Id);

                return new DeleteDealerLevelResponse(true, "Xóa bảng giá theo đại lý thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa bảng giá theo đại lý.");
                return new DeleteDealerLevelResponse(false, "Không thể xóa bảng giá theo đại lý. Vui lòng thử lại.");
            }
        }
    }
}
