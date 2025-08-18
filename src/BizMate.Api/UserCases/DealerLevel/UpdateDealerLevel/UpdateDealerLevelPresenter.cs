using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerLevel.Commands.UpdateDealerLevel;
using System.Net;

namespace BizMate.Api.UserCases.DealerLevel.UpdateDealerLevel
{
    public class UpdateDealerLevelPresenter : IOutputPort<UpdateDealerLevelResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateDealerLevelPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateDealerLevelResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UpdateDealerLevelResponseViewModel(response.DealerLevel))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
