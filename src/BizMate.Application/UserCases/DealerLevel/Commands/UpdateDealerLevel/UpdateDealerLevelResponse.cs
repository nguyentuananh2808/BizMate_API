using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel
{
    public class UpdateDealerLevelResponse : BaseResponse
    {
        public DealerLevelCoreDto DealerLevel { get; }
        public UpdateDealerLevelResponse(DealerLevelCoreDto dealerLevel, bool success = true, string message = null) : base(success, message)
        {
            DealerLevel = dealerLevel;
        }
        public UpdateDealerLevelResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
