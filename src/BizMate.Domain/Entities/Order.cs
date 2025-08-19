namespace BizMate.Domain.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        // 1 = Retail (khách lẻ), 2 = Wholesale (khách sỉ)
        public int CustomerType { get; set; }

        // Nếu là khách sỉ thì có CustomerId
        public Guid? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // Thông tin khách hàng 
        public string CustomerName { get; set; } = default!;
        public string CustomerPhone { get; set; } = default!;
        public string DeliveryAddress { get; set; } = default!;

        public decimal TotalAmount { get; set; }

        // Trạng thái đơn hàng
        public Guid? StatusId { get; set; }
        public Status? Status { get; set; }

        public ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
}
