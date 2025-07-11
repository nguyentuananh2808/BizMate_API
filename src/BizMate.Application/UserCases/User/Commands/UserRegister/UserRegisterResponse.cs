using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.User.Commands.UserRegister
{
    public class UserRegisterResponse : BaseResponse
    {
        public string Email { get; set; }
        public DateTime OtpExpiredAt { get; set; }

        public UserRegisterResponse( string email, DateTime expiredAt)
            : base(success: true, message: "OTP đã được gửi tới email.")
        {
            Email = email;
            OtpExpiredAt = expiredAt;
        }

        public UserRegisterResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }

}
