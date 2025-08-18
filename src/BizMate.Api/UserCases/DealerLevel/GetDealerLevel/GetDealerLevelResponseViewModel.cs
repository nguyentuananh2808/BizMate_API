using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevel
{
    public class GetDealerLevelResponseViewModel
    {
        public DealerLevelCoreDto DealerLevel { get; set; }
        public GetDealerLevelResponseViewModel(DealerLevelCoreDto dealerLevel)
        {
            DealerLevel = dealerLevel;
        }
    }
}
