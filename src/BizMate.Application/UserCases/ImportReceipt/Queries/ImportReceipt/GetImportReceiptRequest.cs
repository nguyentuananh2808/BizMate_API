using MediatR;

namespace BizMate.Application.UserCases.ImportReceipt.Queries.ImportReceipt
{
    public class GetImportReceiptRequest : IRequest<GetImportReceiptResponse>
    {
        public Guid Id { get; set; }

        public GetImportReceiptRequest(Guid id)
        {
            Id = id;
        }
    }
}
