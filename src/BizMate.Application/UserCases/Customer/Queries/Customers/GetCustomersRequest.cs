using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.Customer.Queries.Customers
{
    public class GetCustomersRequest : SearchCore, IRequest<GetCustomersResponse>
    {
        public bool? IsActive { get; set; }
    }
}
