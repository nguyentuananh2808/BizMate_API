using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Queries.UserLogin;
using System.Net;

namespace BizMate.Api.UserCases.User.UserLogin
{
    public sealed class UserLoginPresenter : IOutputPort<UserLoginResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UserLoginPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UserLoginResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UserLoginResponseViewModel(response.AccessToken, response.User))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
