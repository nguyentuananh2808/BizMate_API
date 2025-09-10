using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Order.Commands.UpdateOrder;
using System.Net;

namespace BizMate.Api.UserCases.Order.UpdateOrder
{
    public class UpdateOrderPresenter : IOutputPort<UpdateOrderResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateOrderPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateOrderResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateOrderResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}