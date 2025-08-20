using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptResponse : BaseResponse
    {
        public UpdateStatusImportReceiptResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
