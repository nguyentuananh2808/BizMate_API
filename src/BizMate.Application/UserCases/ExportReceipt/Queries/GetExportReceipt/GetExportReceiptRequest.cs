using MediatR;

namespace BizMate.Application.UserCases.ExportReceipt.Queries.GetExportReceipt
{
    public class GetExportReceiptRequest : IRequest<GetExportReceiptResponse>
    {
        public Guid Id { get; set; }

        public GetExportReceiptRequest(Guid id)
        {
            Id = id;
        }
    }
}
