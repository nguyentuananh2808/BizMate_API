using MediatR;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceRequest : IRequest<CreateDealerPriceResponse>
    {
        public Guid ProductId { get; set; }
        public Guid DealerLevelId { get; set; }
        public decimal Price { get; set; }
    }
}
