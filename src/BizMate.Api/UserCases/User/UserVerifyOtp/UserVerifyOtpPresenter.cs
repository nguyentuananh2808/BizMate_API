using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.User.Commands.UserVerifyOtp;
using System.Net;

namespace BizMate.Api.UserCases.User.UserVerifyOtp
{
    public class UserVerifyOtpPresenter : IOutputPort<UserVerifyOtpResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UserVerifyOtpPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UserVerifyOtpResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UserVerifyOtpResponseViewModel(response.User))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
