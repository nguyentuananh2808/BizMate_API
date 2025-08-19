using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.DealerPrice.Commands.DeleteDealerPrice
{
    public class DeleteDealerPriceRequest : IRequest<DeleteDealerPriceResponse>
    {
        [Required]
        public Guid DealerPriceId { get; set; }
        public DeleteDealerPriceRequest(Guid dealerPriceId)
        {
            DealerPriceId = dealerPriceId;
        }
    }
}
