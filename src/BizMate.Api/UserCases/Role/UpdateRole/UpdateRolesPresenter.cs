
using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Role.Commands.UpdateRole;
using System.Net;

namespace BizMate.Api.UserCases.Role.UpdateRole
{
    public class UpdateRolePresenter : IOutputPort<UpdateRoleResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(UpdateRoleResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new UpdateRoleResponseViewModel(response.Success, response.Message ?? string.Empty));
        }
    }
}
