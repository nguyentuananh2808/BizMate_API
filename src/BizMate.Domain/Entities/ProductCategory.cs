namespace BizMate.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
