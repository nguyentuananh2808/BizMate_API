using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerLevel.Queries.DealerLevel;
using System.Net;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevel
{
    public class GetDealerLevelPresenter : IOutputPort<GetDealerLevelResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetDealerLevelPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetDealerLevelResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetDealerLevelResponseViewModel(response.DealerLevel))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
