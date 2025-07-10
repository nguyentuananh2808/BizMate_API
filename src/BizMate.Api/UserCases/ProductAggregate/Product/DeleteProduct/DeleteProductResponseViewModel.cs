namespace BizMate.Api.UserCases.ProductAggregate.Product.DeleteProduct
{
    public class DeleteProductResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteProductResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
