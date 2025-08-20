using BizMate.Api.Serialization;
using BizMate.Api.UserCases.ImportReceipt.UpdateStatusImportReceipt;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt;
using System.Net;

namespace BizMate.Api.UserCases.StatusImportReceipt.UpdateStatusStatusImportReceipt
{
    public class UpdateStatusImportReceiptPresenter : IOutputPort<UpdateStatusImportReceiptResponse>
    {
        public JsonContentResult ContentResult { get; }

        public UpdateStatusImportReceiptPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(UpdateStatusImportReceiptResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(new UpdateStatusImportReceiptResponseViewModel(false, response.Message))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
