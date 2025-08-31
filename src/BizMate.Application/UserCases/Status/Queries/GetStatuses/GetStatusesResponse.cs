using BizMate.Application.Common.Dto.CoreDto;
using BizMate.Application.Common.Responses;
using System.Text.Json.Serialization;

namespace BizMate.Application.UserCases.Status.Queries.GetStatuses
{
    public class GetStatusesResponse : BaseResponse
    {
        public IEnumerable<StatuseCoreDto> Statuses { get; }

        [JsonConstructor]
        public GetStatusesResponse(IEnumerable<StatuseCoreDto> statuses, bool success = true) : base(success)
        {
            Statuses = statuses;
        }

        public GetStatusesResponse(bool success = false, string message = null) : base(success, message)
        {
        }
    }
}
