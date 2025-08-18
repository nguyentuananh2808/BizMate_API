using MediatR;
using System.ComponentModel.DataAnnotations;


namespace BizMate.Application.UserCases.Customer.Commands.DeleteCustomer
{
    public class DeleteCustomerRequest : IRequest<DeleteCustomerResponse>
    {
        [Required]
        public Guid Id { get; set; }
        public DeleteCustomerRequest(Guid id)
        {
            Id = id;
        }
    }
}
