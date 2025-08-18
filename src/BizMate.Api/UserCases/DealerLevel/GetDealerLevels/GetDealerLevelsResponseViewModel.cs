using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevels
{
    public class GetDealerLevelsResponseViewModel
    {
        public IEnumerable<DealerLevelCoreDto> DealerLevels { get; set; }
        public int TotalCount { get; }
        public GetDealerLevelsResponseViewModel(IEnumerable<DealerLevelCoreDto> dealerLevels, int totalCount)
        {
            DealerLevels = dealerLevels;
            TotalCount = totalCount;
        }
    }
}