using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.InventoryReceipt.Commands.CreateInventoryReceipt;
using System.Net;

namespace BizMate.Api.UserCases.InventoryReceipt.CreateInventoryReceipt
{
    public class CreateInventoryReceiptPresenter : IOutputPort<CreateInventoryReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateInventoryReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateInventoryReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new CreateInventoryReceiptResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
