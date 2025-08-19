using BizMate.Api.Serialization;
using BizMate.Api.UserCases.DealerPrice.DealerPricePrice;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.DealerPrice.Commands.CreateDealerPrice;
using System.Net;

namespace BizMate.Api.UserCases.DealerPrice.CreateDealerPrice
{
    public class CreateDealerPricePresenter : IOutputPort<CreateDealerPriceResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateDealerPricePresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateDealerPriceResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new CreateDealerPriceResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}