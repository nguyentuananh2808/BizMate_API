namespace BizMate.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string ProductCategoryCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
