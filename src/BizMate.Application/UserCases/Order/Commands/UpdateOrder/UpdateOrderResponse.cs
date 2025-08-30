using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Order.Commands.UpdateOrder
{
    public class UpdateOrderResponse : BaseResponse
    {
        public UpdateOrderResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
