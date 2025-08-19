using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice
{
    public class UpdateDealerPriceRequest : IRequest<UpdateDealerPriceResponse>
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        public Guid RowVersion { get; set; } = default!;
        [Required]
        public decimal Price { get; set; } = default!;
    }
}
