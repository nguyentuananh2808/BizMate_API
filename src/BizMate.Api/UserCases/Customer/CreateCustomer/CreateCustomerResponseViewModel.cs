using BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer;

namespace BizMate.Api.UserCases.Customer.CreateCustomer
{
    public class CreateCustomerResponseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public Guid? DealerLevelId { get; set; }

        public CreateCustomerResponseViewModel(CreateCustomerResponse response)
        {
            Id = response.Customer.Id;
            Name = response.Customer.Name;
            Phone = response.Customer.Phone;
            Address = response.Customer.Address;
            DealerLevelId = response.Customer.DealerLevelId;
        }

    }
}
