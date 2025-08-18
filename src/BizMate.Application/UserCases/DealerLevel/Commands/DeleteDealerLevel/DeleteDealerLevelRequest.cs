using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel
{
    public class DeleteDealerLevelRequest : IRequest<DeleteDealerLevelResponse>
    {
        [Required]
        public Guid Id { get; set; }
        public DeleteDealerLevelRequest(Guid id)
        {
            Id = id;
        }
    }
}
