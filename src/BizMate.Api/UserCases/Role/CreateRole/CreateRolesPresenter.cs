using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Role.Commands.CreateRole;
using System.Net;

namespace BizMate.Api.UserCases.Role.CreateRole
{
    public class CreateRolesPresenter : IOutputPort<CreateRoleResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(CreateRoleResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new CreateRolesResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
