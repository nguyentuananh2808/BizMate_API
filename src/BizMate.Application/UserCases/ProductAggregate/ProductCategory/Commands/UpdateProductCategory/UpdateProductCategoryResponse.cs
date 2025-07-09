using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Commands.UpdateProductCategory
{
    public class UpdateProductCategoryResponse : BaseResponse
    {
        public ProductCategoryCoreDto ProdductCategory { get; }
        public UpdateProductCategoryResponse(ProductCategoryCoreDto productCategory, bool success = true, string message = null) : base(success, message)
        {
            ProdductCategory = productCategory;
        }
        public UpdateProductCategoryResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
