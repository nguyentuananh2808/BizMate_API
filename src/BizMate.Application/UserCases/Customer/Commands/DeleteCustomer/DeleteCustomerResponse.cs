using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Customer.Commands.DeleteCustomer
{
    public class DeleteCustomerResponse : BaseResponse
    {
        public DeleteCustomerResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
