using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.User.Commands.UserRegister
{
    public class UserRegisterRequest : IRequest<UserRegisterResponse>
    {
        [Required]
        public string FullName { get; set; } = default!;
        [Required]
        public string Email { get; set; } = default!;
    }
}
