using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.Order.Queries.GetOrders
{
    public class GetOrdersRequest : SearchCore, IRequest<GetOrdersResponse>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<Guid>? StatusIds { get; set; }
    }
}
