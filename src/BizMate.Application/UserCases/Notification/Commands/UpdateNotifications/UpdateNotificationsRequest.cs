using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Notification.Commands.UpdateNotifications
{
    public class UpdateNotificationsRequest : IRequest<UpdateNotificationsResponse>
    {
        [Required]
        public IEnumerable<Guid> NotificationIds { get; set; } = default!;
    }
}
