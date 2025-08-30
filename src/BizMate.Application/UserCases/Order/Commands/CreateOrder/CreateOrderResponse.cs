using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Order.Commands.CreateOrder
{
    public class CreateOrderResponse : BaseResponse
    {
        public CreateOrderResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
