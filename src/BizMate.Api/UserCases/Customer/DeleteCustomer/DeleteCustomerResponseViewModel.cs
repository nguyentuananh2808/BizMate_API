namespace BizMate.Api.UserCases.Customer.DeleteCustomer
{
    public class DeleteCustomerResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteCustomerResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
