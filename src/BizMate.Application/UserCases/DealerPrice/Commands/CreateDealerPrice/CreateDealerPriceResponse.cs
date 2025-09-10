using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice
{
    public class CreateDealerPriceResponse : BaseResponse
    {
        public CreateDealerPriceResponse(bool success = true, string message = null) : base(success, message)
        {
        }
    }
}
