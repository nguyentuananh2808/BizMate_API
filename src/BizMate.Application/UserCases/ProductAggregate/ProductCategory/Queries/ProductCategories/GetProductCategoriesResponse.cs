using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ProductAggregate.ProductCategory.Queries.ProductCategories
{
    public class GetProductCategoriesResponse : BaseResponse
    {
        public IEnumerable<ProductCategoryCoreDto> ProductCategories { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetProductCategoriesResponse(IEnumerable<ProductCategoryCoreDto> productCategories, int totalCount, bool success = true) : base(success)
        {
            ProductCategories = productCategories;
            TotalCount = totalCount;
        }

        public GetProductCategoriesResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
