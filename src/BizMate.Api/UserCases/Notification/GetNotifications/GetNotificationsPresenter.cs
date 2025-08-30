using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Notification.Queries.GetNotifications;
using System.Net;

namespace BizMate.Api.UserCases.Notification.GetNotifications
{
    public class GetNotificationsPresenter : IOutputPort<GetNotificationsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetNotificationsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetNotificationsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetNotificationsResponseViewModel(response.Notifications))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
