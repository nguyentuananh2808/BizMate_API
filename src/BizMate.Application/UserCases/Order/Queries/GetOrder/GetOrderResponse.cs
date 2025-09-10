using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Order.Queries.GetOrder
{
    public class GetOrderResponse : BaseResponse
    {

        public OrderCoreDto Order { get; }
        [JsonConstructor]
        public GetOrderResponse(OrderCoreDto order, bool success = true) : base(success)
        {
            Order = order;
        }
        public GetOrderResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
