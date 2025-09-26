using AutoMapper;
using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BizMate.Application.UserCases.Notification.Queries.GetNotifications
{
    public class GetNotificationsHandler : IRequestHandler<GetNotificationsRequest, GetNotificationsResponse>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserSession _userSession;
        private readonly IMapper _mapper;
        private readonly ILogger<GetNotificationsHandler> _logger;

        #region constructor
        public GetNotificationsHandler(
            INotificationRepository NotificationRepository,
            IUserSession userSession,
            IMapper mapper,
            ILogger<GetNotificationsHandler> logger)
        {
            _notificationRepository = NotificationRepository;
            _userSession = userSession;
            _mapper = mapper;
            _logger = logger;
        }
        #endregion

        public async Task<GetNotificationsResponse> Handle(GetNotificationsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var storeId = _userSession.StoreId;
                var userId = _userSession.UserId;

                var notifications = await _notificationRepository.GetUnreadNotificationsAsync(
                    storeId: storeId,
                    userId: Guid.Parse(userId),
                    cancellationToken);

                var mappedNotifications = _mapper.Map<List<NotificationDto>>(notifications);
                return new GetNotificationsResponse(mappedNotifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn danh sách thông báo.");
                return new GetNotificationsResponse(false, "Không thể tải danh sách thông báo. Vui lòng thử lại.");
            }
        }
    }
}
