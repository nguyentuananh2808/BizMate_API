using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategory
{
    public class GetProductCategoryResponse : BaseResponse
    {
        public ProductCategoryCoreDto ProductCategory { get; }

        [JsonConstructor]
        public GetProductCategoryResponse(ProductCategoryCoreDto productCategory, bool success = true) : base(success)
        {
            ProductCategory = productCategory;
        }

        public GetProductCategoryResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
