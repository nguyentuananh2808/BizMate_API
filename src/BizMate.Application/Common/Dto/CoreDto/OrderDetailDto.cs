using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class OrderDetailDto : BaseCoreEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Unit { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
