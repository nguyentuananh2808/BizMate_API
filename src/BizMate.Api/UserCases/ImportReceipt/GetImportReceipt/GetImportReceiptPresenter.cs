using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt;
using System.Net;

namespace BizMate.Api.UserCases.ImportReceipt.GetImportReceipt
{
    public class GetImportReceiptPresenter : IOutputPort<GetImportReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetImportReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetImportReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetImportReceiptResponseViewModel(response))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
