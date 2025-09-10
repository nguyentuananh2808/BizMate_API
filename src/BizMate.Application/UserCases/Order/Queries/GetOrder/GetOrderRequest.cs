using MediatR;

namespace BizMate.Application.UserCases.Order.Queries.GetOrder
{
    public class GetOrderRequest : IRequest<GetOrderResponse>
    {
        public Guid Id { get; set; }
        public GetOrderRequest(Guid id)
        {
            Id = id;
        }
    }
}
