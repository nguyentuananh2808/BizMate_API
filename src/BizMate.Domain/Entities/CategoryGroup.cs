namespace BizMate.Domain.Entities
{
    public class CategoryGroup : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    }
}
