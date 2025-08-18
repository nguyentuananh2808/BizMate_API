using MediatR;

namespace BizMate.Application.UserCases.DealerLevel.Queries.DealerLevel
{
    public class GetDealerLevelRequest : IRequest<GetDealerLevelResponse>
    {
        public Guid Id { get; set; }
        public GetDealerLevelRequest(Guid id)
        {
            Id = id;
        }
    }
}
