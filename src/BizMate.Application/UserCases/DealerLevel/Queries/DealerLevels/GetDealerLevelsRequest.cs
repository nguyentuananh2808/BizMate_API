using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels
{
    public class GetDealerLevelsRequest : SearchCore, IRequest<GetDealerLevelsResponse>
    {
        public bool? IsActive { get; set; }
    }
}
