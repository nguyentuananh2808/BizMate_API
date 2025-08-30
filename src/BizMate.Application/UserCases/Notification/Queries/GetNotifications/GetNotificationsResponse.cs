using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Notification.Queries.GetNotifications
{
    public class GetNotificationsResponse : BaseResponse
    {
        public IEnumerable<NotificationDto> Notifications { get; }

        [JsonConstructor]
        public GetNotificationsResponse(IEnumerable<NotificationDto> notifications, bool success = true) : base(success)
        {
            Notifications = notifications;
        }

        public GetNotificationsResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
