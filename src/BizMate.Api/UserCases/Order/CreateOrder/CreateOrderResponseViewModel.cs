namespace BizMate.Api.UserCases.Order.CreateOrder
{
    public class CreateOrderResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public CreateOrderResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
