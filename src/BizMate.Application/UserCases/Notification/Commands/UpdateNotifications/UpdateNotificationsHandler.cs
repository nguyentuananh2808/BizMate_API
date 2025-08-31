using BizMate.Application.Common.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.Notification.Commands.UpdateNotifications
{
    public class UpdateNotificationsHandler : IRequestHandler<UpdateNotificationsRequest, UpdateNotificationsResponse>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<UpdateNotificationsHandler> _logger;

        #region constructor
        public UpdateNotificationsHandler(
            INotificationRepository NotificationRepository,
            ILogger<UpdateNotificationsHandler> logger)
        {
            _notificationRepository = NotificationRepository;
            _logger = logger;
        }
        #endregion
        public async Task<UpdateNotificationsResponse> Handle(UpdateNotificationsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await _notificationRepository.MarkAsReadAsync(request.NotificationIds);

                return new UpdateNotificationsResponse(true, "Cập nhật thông báo thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông báo.");
                return new UpdateNotificationsResponse(false, "Không thể cập nhật thông báo. Vui lòng thử lại.");
            }
        }
    }
}
