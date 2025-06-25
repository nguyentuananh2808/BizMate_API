namespace BizMate.Domain.Entities
{
    public class Stock : BaseEntity
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; } 

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
