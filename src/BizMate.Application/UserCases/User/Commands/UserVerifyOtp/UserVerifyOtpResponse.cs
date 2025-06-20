using BizMate.Public.Dto.UserAggregate;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public class UserVerifyOtpResponse : BaseResponse
    {
        public UserCoreDto User { get; }
        public UserVerifyOtpResponse(UserCoreDto user, bool success = true, string message = null) : base(success, message)
        {
            User = user;
        }
        public UserVerifyOtpResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
