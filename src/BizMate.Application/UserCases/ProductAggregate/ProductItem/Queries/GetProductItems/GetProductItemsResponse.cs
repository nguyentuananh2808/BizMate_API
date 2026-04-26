using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.ProductAggregate.ProductItem.Queries.GetProductItems
{
    public class GetProductItemsResponse : BaseResponse
    {
        public IEnumerable<ProductItemCoreDto> ProductItems { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetProductItemsResponse(
            IEnumerable<ProductItemCoreDto> productItems,
            int totalCount,
            bool success = true) : base(success)
        {
            ProductItems = productItems;
            TotalCount = totalCount;
        }

        public GetProductItemsResponse(bool success = false, string message = null) : base(success, message)
        {
            ProductItems = new List<ProductItemCoreDto>();
        }
    }
}
