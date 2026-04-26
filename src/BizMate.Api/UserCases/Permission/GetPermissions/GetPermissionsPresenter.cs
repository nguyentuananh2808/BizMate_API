using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Permission.Queries.GetPermissions;
using System.Net;

namespace BizMate.Api.UserCases.Permission.GetPermissions
{
    public class GetPermissionsPresenter : IOutputPort<GetPermissionsResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetPermissionsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new GetPermissionsResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
