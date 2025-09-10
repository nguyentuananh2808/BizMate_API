using BizMate.Application.UserCases.Order.Queries.GetOrder;

namespace BizMate.Api.UserCases.Order.GetOrder
{
    public class GetOrderResponseViewModel
    {
        public GetOrderResponse Order { get; set; }
        public GetOrderResponseViewModel(GetOrderResponse order)
        {
            Order = order;
        }
    }
}
