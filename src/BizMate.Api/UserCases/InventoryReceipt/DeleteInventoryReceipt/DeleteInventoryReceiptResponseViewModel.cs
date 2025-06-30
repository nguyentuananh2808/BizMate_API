namespace BizMate.Api.UserCases.InventoryReceipt.DeleteInventoryReceipt
{
    public class DeleteInventoryReceiptResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteInventoryReceiptResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
