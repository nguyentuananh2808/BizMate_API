using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Customer.UpdateCustomer
{
    public class UpdateCustomerResponseViewModel
    {
        public CustomerCoreDto Customer { get; set; }
        public UpdateCustomerResponseViewModel(CustomerCoreDto customer)
        {
            Customer = customer;
        }
    }
}
