using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel
{
    public class CreateDealerLevelRequest : IRequest<CreateDealerLevelResponse>
    {
        [Required]
        public string Name { get; set; } = default!;
    }
}
