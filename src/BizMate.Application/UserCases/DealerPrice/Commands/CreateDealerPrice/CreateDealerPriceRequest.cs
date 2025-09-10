using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceRequest : IRequest<CreateDealerPriceResponse>
    {
        [Required]
        public IEnumerable<Guid> ProductIds { get; set; } = default!;
        [Required]
        public Guid DealerLevelId { get; set; } = default!;
    }
}
