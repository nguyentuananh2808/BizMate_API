using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Customer.Commands.UpdateCustomer
{
    public class UpdateCustomerResponse : BaseResponse
    {
        public CustomerCoreDto Customer { get; }
        public UpdateCustomerResponse(CustomerCoreDto customer, bool success = true, string message = null) : base(success, message)
        {
            Customer = customer;
        }
        public UpdateCustomerResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
