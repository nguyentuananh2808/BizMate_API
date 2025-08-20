namespace BizMate.Domain.Entities
{
    public class ExportReceipt : BaseEntity
    {
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }

        public decimal TotalAmount { get; set; }
        public int? PaymentStatus { get; set; }

        public Guid? StatusId { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;

        public ICollection<ExportReceiptDetail> Details { get; set; } = new List<ExportReceiptDetail>();
        public Status? Status { get; set; }
    }
}
