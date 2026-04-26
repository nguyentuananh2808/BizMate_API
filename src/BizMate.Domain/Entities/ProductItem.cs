namespace BizMate.Domain.Entities
{
    public class ProductItem : Base
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string SerialNumber { get; set; } = default!;
        public string? QrRawContent { get; set; }
        public ProductItemStatus Status { get; set; } = ProductItemStatus.PendingImport;

        public Guid? ImportReceiptDetailId { get; set; }
        public ImportReceiptDetail? ImportReceiptDetail { get; set; }

        public Guid? OrderDetailId { get; set; }
        public OrderDetail? OrderDetail { get; set; }

        public DateTime? SoldAt { get; set; }

        public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
    }

    public enum ProductItemStatus
    {
        PendingImport = 0,
        InStock = 1,
        Reserved = 2,
        Sold = 3,
        Returned = 4,
        Defective = 5
    }
}
