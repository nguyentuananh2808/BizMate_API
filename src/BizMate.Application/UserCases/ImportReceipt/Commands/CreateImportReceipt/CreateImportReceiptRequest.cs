using MediatR;

namespace BizMate.Application.UserCases.ImportReceipt.Commands.CreateImportReceipt
{
    public class CreateImportReceiptRequest : IRequest<CreateImportReceiptResponse>
    {
        public string? SupplierName { get; set; }
        public string? DeliveryAddress { get; set; }

        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string? Description { get; set; }
        public ICollection<CreateImportReceiptDetail> Details { get; set; } = new List<CreateImportReceiptDetail>();
    }
    public class CreateImportReceiptDetail
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
