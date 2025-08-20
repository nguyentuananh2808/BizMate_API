using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptHandler : IRequestHandler<UpdateStatusImportReceiptRequest, UpdateStatusImportReceiptResponse>
    {
        private readonly IStatusRepository _statusRepository;
        private readonly IAppMessageService _messageService;
        private readonly IImportReceiptRepository _importReceiptRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<UpdateStatusImportReceiptHandler> _logger;

        #region constructor
        public UpdateStatusImportReceiptHandler(
            IStatusRepository statusRepository,
            IAppMessageService messageService,
            IUserSession userSession,
            IImportReceiptRepository importReceiptRepository,
            ILogger<UpdateStatusImportReceiptHandler> logger)
        {
            _statusRepository = statusRepository;
            _messageService = messageService;
            _userSession = userSession;
            _importReceiptRepository = importReceiptRepository;
            _logger = logger;
        }
        #endregion
        public async Task<UpdateStatusImportReceiptResponse> Handle(UpdateStatusImportReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;
                var role = _userSession.Role;

                #region check StatusImportReceipt exist
                var importReceipt = await _importReceiptRepository.GetByIdAsync(request.Id);
                if (importReceipt == null)
                {
                    var message = _messageService.NotExist(request.Id.ToString());
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Phiếu nhập kho không tồn tại.");
                }
                #endregion

                #region Check rowversion
                if (importReceipt.RowVersion != request.RowVersion)
                    return new UpdateStatusImportReceiptResponse(false, "Dữ liệu đã bị thay đổi bởi người khác. Vui lòng tải lại.");
                #endregion

                #region get status 
                var statusId = await _statusRepository.GetIdByGroupAndCodeAsync(request.Code, "ImportReceipt");
                if (statusId == Guid.Empty)
                {
                    var message = _messageService.NotExist(request.Code);
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Trạng thái không tồn tại.");
                }
                #endregion

                #region check role
                if (role == "Staff" && request.Code == "APPROVED")//Do not allow employees to approve warehouse receipts
                {
                    var message = _messageService.Forbidden();
                    _logger.LogWarning(message);
                    return new UpdateStatusImportReceiptResponse(false, "Bạn không có quyền cập nhật trạng thái phiếu nhập kho.");
                }
                #endregion

                #region update data
                var statusImportReceipt = new UpdateImportReceiptStatusDto
                {
                    Id = request.Id,
                    RowVersion = Guid.NewGuid(),
                    StatusId = statusId,
                    StoreId = storeId,
                    UpdatedBy = Guid.Parse(userId),
                    UpdatedDate = DateTime.UtcNow
                };


                await _importReceiptRepository.UpdateStatusAsync(statusImportReceipt);
                #endregion


                return new UpdateStatusImportReceiptResponse(true, "Cập nhật phiếu nhập kho thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật phiếu nhập kho.");
                return new UpdateStatusImportReceiptResponse(false, "Không thể cập nhật phiếu nhập kho. Vui lòng thử lại.");
            }
        }
    }
}
