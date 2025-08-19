namespace BizMate.Domain.Entities
{
    public class ImportReceiptDetail : BaseCoreEntity
    {
        public Guid ImportReceiptId { get; set; }
        public ImportReceipt ImportReceipt { get; set; } = default!;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Unit { get; set; }
        public int Quantity { get; set; }
    }
}
