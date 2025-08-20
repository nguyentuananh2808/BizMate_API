using MediatR;

namespace BizMate.Application.UserCases.ExportReceipt.Commands.CreateExportReceipt
{
    public class CreateExportReceiptRequest : IRequest<CreateExportReceiptResponse>
    {
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }

        public decimal TotalAmount { get; set; }
        public int? PaymentStatus { get; set; }

        public Guid? StatusId { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public string? Description { get; set; } 

        public ICollection<CreateExportReceiptDetail> Details { get; set; } = new List<CreateExportReceiptDetail>();
    }
    public class CreateExportReceiptDetail
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
