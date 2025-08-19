using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.ImportReceipt.UpdateImportReceipt
{
    public class UpdateImportReceiptResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateImportReceiptResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
