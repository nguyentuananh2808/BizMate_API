using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel
{
    public class DeleteDealerLevelResponse : BaseResponse
    {
        public DeleteDealerLevelResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
