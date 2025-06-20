using MediatR;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public class UserVerifyOtpRequest : IRequest<UserVerifyOtpResponse>
    {
        public string FullName { get; set; }
        public string NameStore { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Otp { get; set; }
    }
}
