using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipts
{
    public class GetExportReceiptsRequest : SearchCore, IRequest<GetExportReceiptsResponse>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
