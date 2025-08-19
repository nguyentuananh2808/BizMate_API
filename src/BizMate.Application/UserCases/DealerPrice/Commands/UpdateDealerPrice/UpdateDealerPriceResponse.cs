using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice
{
    public class UpdateDealerPriceResponse : BaseResponse
    {
        public DealerPriceCoreDto DealerPrice { get; }
        public UpdateDealerPriceResponse(DealerPriceCoreDto dealerPrice, bool success = true, string message = null) : base(success, message)
        {
            DealerPrice = dealerPrice;
        }
        public UpdateDealerPriceResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
