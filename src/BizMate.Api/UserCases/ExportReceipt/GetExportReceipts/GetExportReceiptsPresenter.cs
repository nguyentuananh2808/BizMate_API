using BizMate.Api.Serialization;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts;
using System.Net;

namespace BizMate.Api.UserCases.ExportReceipt.GetExportReceipts
{
    public class GetExportReceiptsPresenter : IOutputPort<GetExportReceiptsResponse>
    {
        public JsonContentResult ContentResult { get; }

        public GetExportReceiptsPresenter()
        {
            ContentResult = new JsonContentResult();
        }

        public void Handle(GetExportReceiptsResponse response)
        {
            ContentResult.StatusCode = (int)(response.Success ? HttpStatusCode.OK : HttpStatusCode.Unauthorized);
            ContentResult.Content = response.Success
                ? CommonJsonSerializer.SerializeObject(
                    new GetExportReceiptsResponseViewModel(response.ExportReceipts, response.TotalCount))
                : CommonJsonSerializer.SerializeObject(response);
        }
    }
}
