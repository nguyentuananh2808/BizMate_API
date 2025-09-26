using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext db)
        {
            _context = db;
        }
        public async Task SendAsync(NotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = dto.UserId,
                OrderId = dto.OrderId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow,
                StoreId = dto.StoreId,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task BroadcastAsync(NotificationDto notification)
        {
            // gửi cho tất cả user trong store
            var users = await _context.Users.Where(u => u.StoreId == notification.StoreId).Select(u => u.Id).ToListAsync();

            foreach (var userId in users)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = userId,
                    Message = notification.Message,
                    Type = notification.Type,
                    StoreId = notification.StoreId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(Guid storeId, Guid userId, CancellationToken cancellationToken)
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            return await _context.Notifications
                .Where(n => n.UserId == userId && n.StoreId == storeId && !n.IsRead)
                .Where(n => n.CreatedAt >= sevenDaysAgo)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }


        public async Task MarkAsReadAsync(IEnumerable<Guid> notificationIds)
        {
            var notifications = await _context.Notifications
                .Where(n => notificationIds.Contains(n.Id))
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

    }

}
