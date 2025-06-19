using BizMate.Application.Common.Responses;
using BizMate.Public.Dto.Identity;
using BizMate.Public.Dto.UserAggregate;

namespace BizMate.Application.UserCases.User.Commands.UserLogin
{
    public class UserLoginResponse : BaseResponse
    {
        public AccessToken AccessToken { get; }
        public UserCoreDto User { get; }
        public UserLoginResponse(AccessToken accessToken, UserCoreDto user, bool success = true, string message = null) : base(success, message)
        {
            AccessToken = accessToken;
            User = user;
        }
        public UserLoginResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
