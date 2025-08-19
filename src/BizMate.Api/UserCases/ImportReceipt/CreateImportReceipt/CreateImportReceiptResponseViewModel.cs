namespace BizMate.Api.UserCases.ImportReceipt.CreateImportReceipt
{
    public class CreateImportReceiptResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public CreateImportReceiptResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
