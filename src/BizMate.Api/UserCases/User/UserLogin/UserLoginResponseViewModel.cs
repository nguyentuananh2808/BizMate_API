using BizMate.Public.Dto.Identity;

namespace BizMate.Api.UserCases.User.UserLogin
{
    public class UserLoginResponseViewModel
    {
        public AccessToken AccessToken { get; }

        public UserLoginResponseViewModel(AccessToken accessToken)
        {
            AccessToken = accessToken;
        }
    }
}
