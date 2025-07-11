using BizMate.Application.Common.Responses;
using BizMate.Public.Dto.Identity;

namespace BizMate.Application.UserCases.User.Queries.UserLogin
{
    public class UserLoginResponse : BaseResponse
    {
        public AccessToken AccessToken { get; }
        public UserLoginResponse(AccessToken accessToken, bool success = true, string message = null) : base(success, message)
        {
            AccessToken = accessToken;
        }
        public UserLoginResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
