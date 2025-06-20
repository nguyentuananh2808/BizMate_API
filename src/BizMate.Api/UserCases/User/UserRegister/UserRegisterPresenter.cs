using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserRegister;
using System.Net;

namespace BizMate.Api.UserCases.User.UserRegister
{
    public class UserRegisterPresenter : IOutputPort<UserRegisterResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UserRegisterPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UserRegisterResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UserRegisterResponseViewModel(response.Email, response.OtpExpiredAt))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
