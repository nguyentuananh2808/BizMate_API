using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.CreateProductCategory
{
    public class CreateProductCategoryResponse : BaseResponse
    {
        public ProductCategoryCoreDto ProductCategoryCoreDto { get; }
        public CreateProductCategoryResponse(ProductCategoryCoreDto productCategoryCoreDto, bool success, string message = null)
            : base(success, message)
        {
            ProductCategoryCoreDto = productCategoryCoreDto;
        }
    }
}
