using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerPrice.Commands.UpdateDealerPrice;
using System.Net;

namespace BizMate.Api.UserCases.DealerPrice.UpdateDealerPrice
{
    public class UpdateDealerPricePresenter : IOutputPort<UpdateDealerPriceResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateDealerPricePresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateDealerPriceResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new UpdateDealerPriceResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
