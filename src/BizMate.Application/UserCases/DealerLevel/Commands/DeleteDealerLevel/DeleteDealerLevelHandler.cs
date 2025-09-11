using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using BizMate.Public.Message;
using BizMate.Domain.Entities;

namespace BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel
{
    public class DeleteDealerLevelHandler : IRequestHandler<DeleteDealerLevelRequest, DeleteDealerLevelResponse>
    {
        private readonly IDealerLevelRepository _DealerLevelRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteDealerLevelHandler> _logger;

        #region constructor
        public DeleteDealerLevelHandler(
            ICustomerRepository customerRepository,
            IUserSession userSession,
            IDealerLevelRepository DealerLevelRepository,
            ILogger<DeleteDealerLevelHandler> logger)
        {
            _userSession = userSession;
            _customerRepository = customerRepository;
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
                    var message = ValidationMessage.LocalizedStrings.DataNotExist;
                    _logger.LogWarning(message);
                    return new DeleteDealerLevelResponse(false, message);
                }
                #endregion

                #region check if DealerLevel is used
                var inUse = await _customerRepository.HasCustomersWithDealerLevelAsync(request.Id);
                if (inUse)
                {
                    var message = ValidationMessage.LocalizedStrings.ExistCustomerInDealerLevel;
                    _logger.LogWarning(message);
                    return new DeleteDealerLevelResponse(false, message);
                }

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
