namespace BizMate.Domain.Entities
{
    public class ImportReceipt : BaseEntity
    {
        public string? SupplierName { get; set; }
        public string? DeliveryAddress { get; set; }

        public decimal TotalAmount { get; set; }

        public Guid? StatusId { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;

        public ICollection<ImportReceiptDetail> Details { get; set; } = new List<ImportReceiptDetail>();
        public Status? Status { get; set; }
    }
}
