using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels
{
    public class GetDealerLevelsResponse : BaseResponse
    {
        public IEnumerable<DealerLevelCoreDto> DealerLevels { get; }
        public int TotalCount { get; }

        [JsonConstructor]
        public GetDealerLevelsResponse(IEnumerable<DealerLevelCoreDto> dealerLevels, int totalCount, bool success = true) : base(success)
        {
            DealerLevels = dealerLevels;
            TotalCount = totalCount;
        }

        public GetDealerLevelsResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
