using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt
{
    public class DeleteInventoryReceiptResponse : BaseResponse
    {
        public DeleteInventoryReceiptResponse(bool success = false, string? message = null)
            : base(success, message)
        {
        }
    }
}
