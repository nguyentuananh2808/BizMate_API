using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoles;
using System.Net;

namespace BizMate.Api.UserCases.Role.GetRoles
{
    public class GetRolesPresenter : IOutputPort<GetRolesResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetRolesResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                response.Success ? (object)response.Roles : response);
        }
    }
}
