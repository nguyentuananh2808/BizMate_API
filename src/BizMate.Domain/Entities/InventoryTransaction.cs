namespace BizMate.Domain.Entities
{
    public class InventoryTransaction : BaseCoreEntity
    {
        public Guid ProductItemId { get; set; }
        public ProductItem ProductItem { get; set; } = default!;

        public InventoryTransactionType Type { get; set; }
        public ProductItemStatus? FromStatus { get; set; }
        public ProductItemStatus ToStatus { get; set; }
        public string? Note { get; set; }
    }

    public enum InventoryTransactionType
    {
        Import = 1,
        Export = 2,
        Return = 3,
        Adjust = 4,
        Reserve = 5,
        Release = 6
    }
}
