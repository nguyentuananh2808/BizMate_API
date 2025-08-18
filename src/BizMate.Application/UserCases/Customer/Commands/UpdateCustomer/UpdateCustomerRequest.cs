using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerRequest : IRequest<UpdateCustomerResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid RowVersion { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public bool IsActive { get; set; }
        public Guid? DealerLevelId { get; set; }
    }
}
