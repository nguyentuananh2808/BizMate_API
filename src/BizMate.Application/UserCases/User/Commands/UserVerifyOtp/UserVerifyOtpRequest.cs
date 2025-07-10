using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.User.Commands.UserVerifyOtp
{
    public class UserVerifyOtpRequest : IRequest<UserVerifyOtpResponse>
    {
        [Required]
        public string Email { get; set; } = default!;

        [Required]
        public string Otp { get; set; } = default!;
    }
}
