using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevel
{
    public class GetDealerLevelResponseViewModel
    {
        public DealerLevelDetailCoreDto DealerLevel { get; set; }
        public GetDealerLevelResponseViewModel(DealerLevelDetailCoreDto dealerLevel)
        {
            DealerLevel = dealerLevel;
        }
    }
}
