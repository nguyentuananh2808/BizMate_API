using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipt;
using System.Net;

namespace BizMate.Api.UserCases.ExportReceipt.GetExportReceipt
{
    public class GetExportReceiptPresenter : IOutputPort<GetExportReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetExportReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetExportReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetExportReceiptResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
