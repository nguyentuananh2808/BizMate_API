using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder
{
    public class UpdateStatusOrderResponse : BaseResponse
    {
        public UpdateStatusOrderResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }

}
