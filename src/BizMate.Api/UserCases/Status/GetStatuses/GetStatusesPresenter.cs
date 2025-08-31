using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Status.Queries.GetStatuses;
using System.Net;

namespace BizMate.Api.UserCases.Status.GetStatuses
{
    public class GetStatusesPresenter : IOutputPort<GetStatusesResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetStatusesPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetStatusesResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetStatusesResponseViewModel(response.Statuses))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
