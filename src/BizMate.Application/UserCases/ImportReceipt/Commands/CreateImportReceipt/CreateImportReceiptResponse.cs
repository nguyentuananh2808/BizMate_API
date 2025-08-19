using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt
{
    public class CreateImportReceiptResponse : BaseResponse
    {
        public CreateImportReceiptResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
