using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerLevel.Commands.DeleteDealerLevel;
using System.Net;

namespace BizMate.Api.UserCases.DealerLevel.DeleteDealerLevel
{
    public class DeleteDealerLevelPresenter : IOutputPort<DeleteDealerLevelResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteDealerLevelPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteDealerLevelResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteDealerLevelResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
