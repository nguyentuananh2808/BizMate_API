using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerPrice.Commands.DeleteDealerPrice;
using System.Net;

namespace BizMate.Api.UserCases.DealerPrice.DeleteDealerPrice
{
    public class DeleteDealerPricePresenter : IOutputPort<DeleteDealerPriceResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteDealerPricePresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteDealerPriceResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteDealerPriceResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
