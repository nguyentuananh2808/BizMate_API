using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.InventoryReceipt.Commands.DeleteCreateInventoryReceipt;
using System.Net;

namespace BizMate.Api.UserCases.InventoryReceipt.DeleteInventoryReceipt
{
    public class DeleteInventoryReceiptPresenter : IOutputPort<DeleteInventoryReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public DeleteInventoryReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(DeleteInventoryReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new DeleteInventoryReceiptResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
