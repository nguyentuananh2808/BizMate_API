using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevel
{
    public class GetDealerLevelResponse : BaseResponse
    {
        public DealerLevelDetailCoreDto DealerLevel { get; }
        [JsonConstructor]
        public GetDealerLevelResponse(DealerLevelDetailCoreDto dealerLevel, bool success = true) : base(success)
        {
            DealerLevel = dealerLevel;
        }
        public GetDealerLevelResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
