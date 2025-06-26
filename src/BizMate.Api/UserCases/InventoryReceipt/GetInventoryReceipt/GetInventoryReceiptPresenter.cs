using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.InventoryReceipt.Queries.InventoryReceipt;
using System.Net;

namespace BizMate.Api.UserCases.InventoryReceipt.GetInventoryReceipt
{
    public class GetInventoryReceiptPresenter : IOutputPort<GetInventoryReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetInventoryReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetInventoryReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetInventoryReceiptResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}