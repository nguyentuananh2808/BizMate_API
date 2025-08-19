using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ImportReceipt.Commands.UpdateImportReceipt;
using System.Net;

namespace BizMate.Api.UserCases.ImportReceipt.UpdateImportReceipt
{
    public class UpdateImportReceiptPresenter : IOutputPort<UpdateImportReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateImportReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateImportReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateImportReceiptResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
