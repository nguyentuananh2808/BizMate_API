using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerPrice.Commands.DeleteDealerPrice
{
    public class DeleteDealerPriceResponse : BaseResponse
    {
        public DeleteDealerPriceResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
