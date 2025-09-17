namespace BizMate.Domain.Entities
{
    public class OrderDetail : BaseCoreEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Unit { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

    }
}
