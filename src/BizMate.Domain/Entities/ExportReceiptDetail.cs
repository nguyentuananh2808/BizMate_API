namespace BizMate.Domain.Entities
{
    public class ExportReceiptDetail : BaseCoreEntity
    {
        public Guid ExportReceiptId { get; set; }
        public ExportReceipt ExportReceipt { get; set; } = default!;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Unit { get; set; }
        public int Quantity { get; set; }
    }
}
