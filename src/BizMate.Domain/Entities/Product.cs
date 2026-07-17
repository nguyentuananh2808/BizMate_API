namespace BizMate.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int Unit { get; set; } = default!;
        public decimal? CostPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ImageUrl { get; set; }
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = default!;
        public bool IsDiscountable { get; set; } = true;
        public decimal? DiscountPercent { get; set; }

        /// <summary>
        /// Indicates whether inventory is tracked by physical serial number instead of quantity only.
        /// </summary>
        public bool IsSerialTracked { get; set; } = false;

        public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    }
}
