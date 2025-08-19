using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceResponse : BaseResponse
    {
        public DealerPriceCoreDto DealerPrice { get; }
        public CreateDealerPriceResponse(DealerPriceCoreDto dealerPrice, bool success = true, string message = null) : base(success, message)
        {
            DealerPrice = dealerPrice;
        }
        public CreateDealerPriceResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
