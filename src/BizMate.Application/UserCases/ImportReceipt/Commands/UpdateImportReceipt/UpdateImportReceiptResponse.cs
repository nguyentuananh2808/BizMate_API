using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt
{
    public class UpdateImportReceiptResponse : BaseResponse
    {
        public UpdateImportReceiptResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
