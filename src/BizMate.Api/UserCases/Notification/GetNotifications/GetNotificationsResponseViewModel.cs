using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Notification.GetNotifications
{
    public class GetNotificationsResponseViewModel
    {

        public IEnumerable<NotificationDto> Notifications { get; set; }
        public GetNotificationsResponseViewModel(IEnumerable<NotificationDto> notifications)
        {
            Notifications = notifications;
        }
    }
}
