//using BizMate.Application.Common.Interfaces.Repositories;
//using BizMate.Application.Common.Security;
//using MediatR;
//using Microsoft.Extensions.Localization;
//using Microsoft.Extensions.Logging;
//using BizMate.Application.Resources;
//using BizMate.Application.Common.Interfaces;

//namespace BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt
//{
//    public class DeleteInventoryReceiptHandler : IRequestHandler<DeleteInventoryReceiptRequest, DeleteInventoryReceiptResponse>
//    {
//        private readonly IAppMessageService _messageService;
//        private readonly IInventoryReceiptRepository _inventoryRepository;
//        private readonly IUserSession _userSession;
//        private readonly ILogger<DeleteInventoryReceiptHandler> _logger;
//        public DeleteInventoryReceiptHandler(
//            IAppMessageService messageService,
//            IInventoryReceiptRepository inventoryRepository,
//            IUserSession userSession,
//            ILogger<DeleteInventoryReceiptHandler> logger)
//        {
//            _messageService = messageService;
//            _inventoryRepository = inventoryRepository;
//            _userSession = userSession;
//            _logger = logger;
//        }

//        public async Task<DeleteInventoryReceiptResponse> Handle(DeleteInventoryReceiptRequest request, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var storeId = _userSession.StoreId;

//                var receipt = await _inventoryRepository.GetByIdAsync(request.Id);
//                if (receipt == null || receipt.StoreId != storeId)
//                {
//                    var message = _messageService.NotExist(request.Id);
//                    _logger.LogWarning(message);
//                    return new DeleteInventoryReceiptResponse(false, message);
//                }

//                await _inventoryRepository.DeleteAsync(request.Id);
//                return new DeleteInventoryReceiptResponse(true, "Xóa phiếu kho thành công.");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Lỗi khi xóa phiếu kho.");
//                return new DeleteInventoryReceiptResponse(false, "Không thể xóa phiếu kho. Vui lòng thử lại.");
//            }
//        }
//    }
//}
