using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Notification.Queries.GetNotifications
{
    public class GetNotificationsRequest : IRequest<GetNotificationsResponse>
    {
        [Required]
        public  DateTime lastChecked { get; set; }
    }
}
