namespace BizMate.Api.UserCases.Order.UpdateOrder
{
    public class UpdateOrderResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateOrderResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
