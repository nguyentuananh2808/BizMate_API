namespace BizMate.Api.UserCases.Order.UpdateStatusOrder
{
    public class UpdateStatusOrderResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateStatusOrderResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
