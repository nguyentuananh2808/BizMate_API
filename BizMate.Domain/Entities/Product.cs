namespace BizMate.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string SKU { get; set; } = default!;
        public int Quantity { get; set; }
        public string Unit { get; set; } = default!;// ví dụ: "cái", "kg", "thùng"
        public string? ImageUrl { get; set; } 

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
    }

}
