using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Order.Queries.GetOrders
{
    public class GetOrdersResponse : BaseResponse
    {
        public IEnumerable<OrderCoreDto> Orders { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetOrdersResponse(IEnumerable<OrderCoreDto> orders, int totalCount, bool success = true) : base(success)
        {
            Orders = orders;
            TotalCount = totalCount;
        }

        public GetOrdersResponse(bool success = false, string? message = null) : base(success, message)
        {
        }
    }
}
