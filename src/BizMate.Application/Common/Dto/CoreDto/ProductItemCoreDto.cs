using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class ProductItemCoreDto : Base
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductCode { get; set; }
        public string SerialNumber { get; set; } = default!;
        public int Status { get; set; }
        public string StatusName { get; set; } = default!;
        public Guid? ImportReceiptDetailId { get; set; }
        public Guid? OrderDetailId { get; set; }
        public DateTime? SoldAt { get; set; }
    }
}
