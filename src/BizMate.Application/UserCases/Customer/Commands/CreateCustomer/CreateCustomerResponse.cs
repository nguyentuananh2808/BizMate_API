using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;


namespace BizMate.Application.UserCases.CustomerAggregate.Customer.Commands.CreateCustomer
{
    public class CreateCustomerResponse : BaseResponse
    {
        public CustomerCoreDto Customer { get; }
        public CreateCustomerResponse(CustomerCoreDto customer, bool success = true, string message = null) : base(success, message)
        {
            Customer = customer;
        }
        public CreateCustomerResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }

}
