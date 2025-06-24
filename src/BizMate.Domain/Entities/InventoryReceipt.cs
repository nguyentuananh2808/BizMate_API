namespace BizMate.Domain.Entities
{
    public class InventoryReceipt : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string Type { get; set; } = default!; // "Import", "Export", "Adjust"

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;
        public ICollection<InventoryReceiptDetail> Details { get; set; } = new List<InventoryReceiptDetail>();

    }

}
