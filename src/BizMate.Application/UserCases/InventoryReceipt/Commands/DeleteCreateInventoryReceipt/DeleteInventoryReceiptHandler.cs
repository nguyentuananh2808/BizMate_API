using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using BizMate.Public.Message;
using MediatR;
using _InventoryReceipt = BizMate.Domain.Entities.InventoryReceipt;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt
{
    public class DeleteInventoryReceiptHandler : IRequestHandler<DeleteInventoryReceiptRequest, DeleteInventoryReceiptResponse>
    {
        private readonly IInventoryReceiptRepository _inventoryRepository;
        private readonly IUserSession _userSession;
        private readonly ILogger<DeleteInventoryReceiptHandler> _logger;
        private readonly IStringLocalizer<DeleteInventoryReceiptHandler> _localizer;

        public DeleteInventoryReceiptHandler(
            IInventoryReceiptRepository inventoryRepository,
            IUserSession userSession,
            ILogger<DeleteInventoryReceiptHandler> logger,
            IStringLocalizer<DeleteInventoryReceiptHandler> localizer)
        {
            _inventoryRepository = inventoryRepository;
            _userSession = userSession;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<DeleteInventoryReceiptResponse> Handle(DeleteInventoryReceiptRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;

                var receipt = await _inventoryRepository.GetByIdAsync(request.Id);
                if (receipt == null || receipt.StoreId != storeId)
                {
                    var message = CommonAppMessageUtils.NotExist<_InventoryReceipt>(request.Id, _localizer);
                    _logger.LogWarning(message);
                    return new DeleteInventoryReceiptResponse(false, message);
                }

                await _inventoryRepository.DeleteAsync(request.Id);
                return new DeleteInventoryReceiptResponse(true, _localizer["Xóa phiếu kho thành công."]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa phiếu kho.");
                return new DeleteInventoryReceiptResponse(false, _localizer["Không thể xóa phiếu kho. Vui lòng thử lại."]);
            }
        }
    }
}
