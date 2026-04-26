using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.RoleAggregate.Role.Queries.GetRoleDetail;
using System.Net;

namespace BizMate.Api.UserCases.Role.GetRoleDetail
{
    public class GetRoleDetailsPresenter : IOutputPort<GetRoleDetailResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetRoleDetailResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                response.Success ? (object)response.Role! : response);
        }
    }
}
