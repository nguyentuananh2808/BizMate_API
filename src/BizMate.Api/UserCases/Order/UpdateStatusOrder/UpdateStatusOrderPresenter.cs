using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.Order.Commands.UpdateStatusOrder;
using System.Net;

namespace BizMate.Api.UserCases.Order.UpdateStatusOrder
{
    public class UpdateStatusOrderPresenter : IOutputPort<UpdateStatusOrderResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateStatusOrderPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateStatusOrderResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateStatusOrderResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
