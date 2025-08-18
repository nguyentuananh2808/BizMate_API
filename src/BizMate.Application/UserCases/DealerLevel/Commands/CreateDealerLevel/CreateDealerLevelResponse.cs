using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;

namespace BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel
{
    public class CreateDealerLevelResponse : BaseResponse
    {
        public DealerLevelCoreDto DealerLevel { get; }
        public CreateDealerLevelResponse(DealerLevelCoreDto dealerLevel, bool success = true, string message = null) : base(success, message)
        {
            DealerLevel = dealerLevel;
        }
        public CreateDealerLevelResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
