using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Order.Commands.CreateOrder;
using System.Net;

namespace BizMate.Api.UserCases.Order.CreateOrder
{
    public class CreateOrderPresenter : IOutputPort<CreateOrderResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateOrderPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateOrderResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new CreateOrderResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
