using BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice;

namespace BizMate.Api.UserCases.DealerPrice.DealerPricePrice
{
    public class CreateDealerPriceResponseViewModel
    {

        public Guid Id { get; set; }
        public Guid DealerLevelId { get; set; }
        public Guid ProductId { get; set; }
        public decimal Price { get; set; }


        public CreateDealerPriceResponseViewModel(CreateDealerPriceResponse response)
        {
            Id = response.DealerPrice.Id;
            Price = response.DealerPrice.Price;
            ProductId = response.DealerPrice.ProductId;
            DealerLevelId = response.DealerPrice.DealerLevelId;
        }

    }
}
