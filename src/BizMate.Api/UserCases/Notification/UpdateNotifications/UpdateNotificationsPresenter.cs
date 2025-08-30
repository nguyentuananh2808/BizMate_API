using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Notification.Commands.UpdateNotifications;
using System.Net;

namespace BizMate.Api.UserCases.Notification.UpdateNotifications
{
    public class UpdateNotificationsPresenter : IOutputPort<UpdateNotificationsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateNotificationsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateNotificationsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateNotificationsResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
