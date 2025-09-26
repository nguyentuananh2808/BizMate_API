using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Interfaces.Repositories
{
    public interface INotificationRepository
    {
        Task SendAsync(NotificationDto notification);
        Task BroadcastAsync(NotificationDto notification);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid storeId, Guid userId,CancellationToken cancellationToken);
        Task MarkAsReadAsync(IEnumerable<Guid> notificationIds);
    }

}
