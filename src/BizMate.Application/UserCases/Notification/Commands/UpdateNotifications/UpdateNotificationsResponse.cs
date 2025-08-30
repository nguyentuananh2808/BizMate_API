using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.Notification.Commands.UpdateNotifications
{
    public class UpdateNotificationsResponse : BaseResponse
    {
        public UpdateNotificationsResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
