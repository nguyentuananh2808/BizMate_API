
using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerLevel.Queries.DealerLevels;
using System.Net;

namespace BizMate.Api.UserCases.DealerLevel.GetDealerLevels
{
    public class GetDealerLevelsPresenter : IOutputPort<GetDealerLevelsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetDealerLevelsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetDealerLevelsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetDealerLevelsResponseViewModel(response.DealerLevels, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
