using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.DealerPrice.UpdateDealerPrice
{
    public class UpdateDealerPriceResponseViewModel
    {
        public DealerPriceCoreDto DealerPrice { get; set; }
        public UpdateDealerPriceResponseViewModel(DealerPriceCoreDto dealerPrice)
        {
            DealerPrice = dealerPrice;
        }
    }
}
