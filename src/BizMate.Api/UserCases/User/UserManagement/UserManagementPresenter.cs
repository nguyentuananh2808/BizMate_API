using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserManagement;
using BizMate.Application.UserCases.User.Queries.UserManagement;
using System.Net;

namespace BizMate.Api.UserCases.User.UserManagement
{
    public class GetUsersPresenter : IOutputPort<GetUsersResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetUsersResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new GetUsersResponseViewModel(response.Users, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }

    public class GetUserDetailPresenter : IOutputPort<GetUserDetailResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(GetUserDetailResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.NotFound);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                response.Success ? (object)response.User! : response);
        }
    }

    public class UserMutationPresenter : IOutputPort<UserMutationResponse>
    {
        public JsonContentResult ContentResult { get; } = new();

        public void Handle(UserMutationResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = CommonJsonSerializer.SerializeObject(
                new UserMutationResponseViewModel(
                    response.Success,
                    response.Message ?? string.Empty,
                    response.UserId));
        }
    }
}
