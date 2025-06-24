namespace BizMate.Domain.Entities
{
    public class InventoryReceiptDetail : BaseEntity
    {
        public Guid InventoryReceiptId { get; set; }
        public InventoryReceipt InventoryReceipt { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
    }

}
