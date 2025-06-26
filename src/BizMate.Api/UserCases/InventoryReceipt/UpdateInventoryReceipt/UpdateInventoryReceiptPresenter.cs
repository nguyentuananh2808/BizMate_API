using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.InventoryReceipt.Commands.UpdateInventoryReceipt;
using System.Net;

namespace BizMate.Api.UserCases.InventoryReceipt.UpdateInventoryReceipt
{
    public class UpdateInventoryReceiptPresenter : IOutputPort<UpdateInventoryReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateInventoryReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateInventoryReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateInventoryReceiptResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
