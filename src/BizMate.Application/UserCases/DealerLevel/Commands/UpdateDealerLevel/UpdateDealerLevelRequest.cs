using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel
{
    public class UpdateDealerLevelRequest : IRequest<UpdateDealerLevelResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid RowVersion { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
    }
}
