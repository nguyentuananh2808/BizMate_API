using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Customer.GetCustomer
{
    public class GetCustomerResponseViewModel
    {
        public CustomerCoreDto Customer { get; set; }
        public GetCustomerResponseViewModel(CustomerCoreDto customer)
        {
            Customer = customer;
        }
    }
}
