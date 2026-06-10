using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserPermissions;
using BizMate.Application.UserCases.User.Queries.UserPermissions;
using System.Net;

namespace BizMate.Api.UserCases.User.UserPermissions
{
    public class GetUserPermissionsPresenter : IOutputPort<GetUserPermissionsResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetUserPermissionsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                response.Success ? (object)response.Data! : response);
        }
    }

    public class UserPermissionMutationPresenter : IOutputPort<UserPermissionMutationResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(UserPermissionMutationResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new UserPermissionMutationResponseViewModel(
                    response.Success,
                    response.Message ?? string.Empty,
                    response.PermissionIds));
        }
    }
}
