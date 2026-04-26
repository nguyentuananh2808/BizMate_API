using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Role.Commands.DeleteRole;
using System.Net;

namespace BizMate.Api.UserCases.Role.DeleteRole
{
    public class DeleteRolesPresenter : IOutputPort<DeleteRoleResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(DeleteRoleResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new DeleteRolesResponseViewModel(response.Success, response.Message ?? string.Empty));
        }
    }
}
