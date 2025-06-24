namespace BizMate.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int Unit { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }
}
