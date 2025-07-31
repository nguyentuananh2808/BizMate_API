namespace BizMate.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int Unit { get; set; } = default!;
        public decimal? CostPrice { get; set; } // Giá nhập 
        public decimal? SalePrice { get; set; } // Giá bán
        public string? ImageUrl { get; set; }
        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public Guid? ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; } = default!;
        // Mở rộng cho tính năng khuyến mãi
        public bool IsDiscountable { get; set; } = true;            // Có được giảm giá không?
        public decimal? DiscountPercent { get; set; }               // % giảm giá mặc định nếu có

    }
}
