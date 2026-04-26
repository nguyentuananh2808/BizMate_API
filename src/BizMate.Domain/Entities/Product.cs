// FILE: src/BizMate.Domain/Entities/Product.cs
// Thêm field IsSerialTracked vào Product hiện có

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
        /// Bật = quản lý từng đơn vị vật lý qua SerialNumber/MAC.
        /// Tắt = chỉ quản lý số lượng (hành vi cũ, mặc định).
        /// </summary>
        public bool IsSerialTracked { get; set; } = false;

        // Navigation sang ProductItem (chỉ có data khi IsSerialTracked = true)
        public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
    }
}
