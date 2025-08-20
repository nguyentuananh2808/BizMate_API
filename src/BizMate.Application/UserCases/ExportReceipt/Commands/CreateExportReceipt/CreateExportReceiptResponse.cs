using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ExportReceipt.Commands.CreateExportReceipt
{
    public class CreateExportReceiptResponse : BaseResponse
    {
        public CreateExportReceiptResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
