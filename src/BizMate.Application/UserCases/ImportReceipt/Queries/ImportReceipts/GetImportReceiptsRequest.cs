using BizMate.Application.Common.Requests;
using MediatR;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipts
{
    public class GetImportReceiptsRequest : SearchCore, IRequest<GetImportReceiptsResponse>
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public IEnumerable<Guid>? StatusIds { get; set; }
    }
}
