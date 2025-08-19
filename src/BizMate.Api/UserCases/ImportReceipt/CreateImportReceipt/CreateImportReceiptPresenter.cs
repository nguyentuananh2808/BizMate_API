using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt;
using System.Net;

namespace BizMate.Api.UserCases.ImportReceipt.CreateImportReceipt
{
    public class CreateImportReceiptPresenter : IOutputPort<CreateImportReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public CreateImportReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(CreateImportReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new CreateImportReceiptResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
