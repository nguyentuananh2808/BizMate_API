using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerLevel.Commands.CreateDealerLevel;
using System.Net;

namespace BizMate.Api.UserCases.DealerLevel.CreateDealerLevel
{
    public class CreateDealerLevelPresenter : IOutputPort<CreateDealerLevelResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateDealerLevelPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateDealerLevelResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new CreateDealerLevelResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}