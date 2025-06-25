using BizMate.Application.Common.Dto.UserAggregate;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Product.Queries.Product
{
    public class GetProductResponse : BaseResponse
    {
        public ProductCoreDto Product { get; }

        [JsonConstructor]
        public GetProductResponse(ProductCoreDto product, bool success = true) : base(success)
        {
            Product = product;
        }

        public GetProductResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
