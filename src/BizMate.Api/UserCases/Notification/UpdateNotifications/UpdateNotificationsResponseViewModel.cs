namespace BizMate.Api.UserCases.Notification.UpdateNotifications
{
    public class UpdateNotificationsResponseViewModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public UpdateNotificationsResponseViewModel(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
