using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class OrderCoreDto : BaseEntity
    {
        public DateTime OrderDate { get; set; } 

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
        public Guid StatusId { get; set; }
        public string? StatusName { get; set; }

        public Status Status { get; set; } = default!;
        public void RecalculateTotal()
        {
            TotalAmount = Details?.Sum(d => d.Total) ?? 0;
        }
        public ICollection<OrderDetailDto> Details { get; set; } = new List<OrderDetailDto>();
    }
}
