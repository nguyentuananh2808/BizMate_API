using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class ProductsResponse : BaseResponse
    {
        public IEnumerable<ProductCoreDto> Products { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public ProductsResponse(IEnumerable<ProductCoreDto> products, int totalCount, bool success = true) : base(success)
        {
            Products = products;
            TotalCount = totalCount;
        }

        public ProductsResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
