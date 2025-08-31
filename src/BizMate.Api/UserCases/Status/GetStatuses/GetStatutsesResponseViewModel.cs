using BizMate.Application.Common.Dto.CoreDto;

namespace BizMate.Api.UserCases.Status.GetStatuses
{
    public class GetStatusesResponseViewModel
    {
        public IEnumerable<StatuseCoreDto> Statuses { get; set; }
        public GetStatusesResponseViewModel(IEnumerable<StatuseCoreDto> statuses)
        {
            Statuses = statuses;
        }
    }
}
