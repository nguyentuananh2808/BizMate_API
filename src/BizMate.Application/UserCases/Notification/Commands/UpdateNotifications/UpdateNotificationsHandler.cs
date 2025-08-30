using AutoMapper;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;

namespace BizMate.Application.UserCases.Notification.Commands.UpdateNotifications
{
    public class UpdateNotificationsHandler : IRequestHandler<UpdateNotificationsRequest, UpdateNotificationsResponse>
    {
        private readonly IAppMessageService _messageService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserSession _userSession;
        private readonly QueryFactory _db;
        private readonly ILogger<UpdateNotificationsHandler> _logger;
        private readonly IMapper _mapper;

        #region constructor
        public UpdateNotificationsHandler(
            IAppMessageService messageService,
            IUserSession userSession,
            INotificationRepository NotificationRepository,
            QueryFactory db,
            ILogger<UpdateNotificationsHandler> logger,
            IMapper mapper)
        {
            _messageService = messageService;
            _userSession = userSession;
            _notificationRepository = NotificationRepository;
            _db = db;
            _logger = logger;
            _mapper = mapper;
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
