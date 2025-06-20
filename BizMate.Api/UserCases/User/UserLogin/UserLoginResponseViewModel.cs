using BizMate.Public.Dto.Identity;
using BizMate.Public.Dto.UserAggregate;

namespace BizMate.Api.UserCases.User.UserLogin
{
    public class UserLoginResponseViewModel
    {
        public AccessToken AccessToken { get; }
        public UserCoreDto User { get; }

        public UserLoginResponseViewModel(AccessToken accessToken, UserCoreDto user)
        {
            AccessToken = accessToken;
            User = user;
        }
    }
}
