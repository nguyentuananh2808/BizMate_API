using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer
{
    public class CreateCustomerRequest : IRequest<CreateCustomerResponse>
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Phone { get; set; } = default!;
        [Required]
        public string Address { get; set; } = default!;
        public Guid? DealerLevelId { get; set; }
    }
}
