using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.User.Commands.UserLogin
{
    public class UserLoginRequest : IRequest<UserLoginResponse>
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
