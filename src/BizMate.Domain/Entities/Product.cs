namespace BizMate.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int Unit { get; set; } = default!;
        public decimal? CostPrice { get; set; } // Giá nhập 
        public decimal? SalePrice { get; set; } // Giá bán
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        // Mở rộng cho tính năng khuyến mãi
        public bool IsDiscountable { get; set; } = true;            // Có được giảm giá không?
        public decimal? DiscountPercent { get; set; }               // % giảm giá mặc định nếu có

    }
}
