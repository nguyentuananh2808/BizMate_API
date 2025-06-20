using BizMate.Public.Dto.UserAggregate;

namespace BizMate.Api.UserCases.User.UserVerifyOtp
{
    public class UserVerifyOtpResponseViewModel
    {
        public UserCoreDto User { get; }

        public UserVerifyOtpResponseViewModel( UserCoreDto user)
        {
            User = user;
        }
    }
}
