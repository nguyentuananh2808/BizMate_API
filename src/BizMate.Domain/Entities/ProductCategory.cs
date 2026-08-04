namespace BizMate.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = default!;
        public Guid? CategoryGroupId { get; set; }
        public CategoryGroup? CategoryGroup { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
