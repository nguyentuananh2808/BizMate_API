using MediatR;
using System.ComponentModel.DataAnnotations;

namespace BizMate.Application.UserCases.Status.Queries.GetStatuses
{
    public class GetStatusesRequest : IRequest<GetStatusesResponse>
    {
        [Required]
        public string Group { get; set; } = default!;
    }
}
