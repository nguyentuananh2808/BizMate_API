using BizMate.Application.Common.Responses;
using MediatR;

namespace BizMate.Application.UserCases.User.Commands.UserPassword
{
    public class ForgotPasswordRequest : IRequest<ForgotPasswordResponse>
    {
        public string Email { get; set; } = default!;
    }

    public class ForgotPasswordResponse : BaseResponse
    {
        public string? Email { get; set; }
        public DateTime? OtpExpiredAt { get; set; }

        public ForgotPasswordResponse(string email, DateTime otpExpiredAt)
            : base(true, "OTP đặt lại mật khẩu đã được gửi tới email.")
        {
            Email = email;
            OtpExpiredAt = otpExpiredAt;
        }

        public ForgotPasswordResponse(bool success = false, string? message = null)
            : base(success, message) { }
    }

    public class ResetPasswordRequest : IRequest<ResetPasswordResponse>
    {
        public string Email { get; set; } = default!;
        public string Otp { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }

    public class ResetPasswordResponse : BaseResponse
    {
        public ResetPasswordResponse(bool success = true, string? message = null)
            : base(success, message) { }
    }
}
