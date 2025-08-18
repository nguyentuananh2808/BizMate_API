using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.DealerLevel.UpdateDealerLevel
{
    public class UpdateDealerLevelResponseViewModel
    {
        public DealerLevelCoreDto DealerLevel { get; set; }
        public UpdateDealerLevelResponseViewModel(DealerLevelCoreDto dealerLevel)
        {
            DealerLevel = dealerLevel;
        }
    }
}
