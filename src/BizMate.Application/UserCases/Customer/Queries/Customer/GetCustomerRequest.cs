using MediatR;

namespace BizMate.Application.UserCases.Customer.Queries.Customer
{
    public class GetCustomerRequest : IRequest<GetCustomerResponse>
    {
        public Guid Id { get; set; }
        public GetCustomerRequest(Guid id)
        {
            Id = id;
        }
    }
}
