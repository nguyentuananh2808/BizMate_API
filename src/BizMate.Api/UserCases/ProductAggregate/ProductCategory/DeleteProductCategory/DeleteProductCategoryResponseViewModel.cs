namespace BizMate.Api.UserCases.ProductAggregate.ProductCategory.DeleteProductCategory
{
    public class DeleteProductCategoryResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public DeleteProductCategoryResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
