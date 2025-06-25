using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Product.Queries.Products
{
    public class GetProductsResponse : BaseResponse
    {
        public IEnumerable<ProductCoreDto> Products { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetProductsResponse(IEnumerable<ProductCoreDto> products, int totalCount, bool success = true) : base(success)
        {
            Products = products;
            TotalCount = totalCount;
        }

        public GetProductsResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
