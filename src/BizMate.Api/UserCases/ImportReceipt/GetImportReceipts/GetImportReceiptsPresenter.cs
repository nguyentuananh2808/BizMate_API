using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts;
using System.Net;

namespace BizMate.Api.UserCases.ImportReceipt.GetImportReceipts
{
    public class GetImportReceiptsPresenter : IOutputPort<GetImportReceiptsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetImportReceiptsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetImportReceiptsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetImportReceiptsResponseViewModel(response.ImportReceipts, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
