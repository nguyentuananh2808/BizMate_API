namespace BizMate.Api.UserCases.ImportReceipt.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateStatusImportReceiptResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
