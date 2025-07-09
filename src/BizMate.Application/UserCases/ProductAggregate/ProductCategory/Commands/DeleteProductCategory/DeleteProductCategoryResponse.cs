using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.DeleteProductCategory
{
    public class DeleteProductCategoryResponse : BaseResponse
    {
        public DeleteProductCategoryResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
