using MediatR;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.UpdateStatusImportReceipt
{
    public class UpdateStatusImportReceiptRequest : IRequest<UpdateStatusImportReceiptResponse>
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public Guid RowVersion { get; set; }
    }
}
