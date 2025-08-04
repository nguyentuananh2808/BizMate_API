using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class InventoryReceiptCoreDto : BaseEntity
    {
        public DateTime Date { get; set; }
        public int Type { get; set; }

        public string StoreName { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = default!;

        public string? SupplierName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusCode { get; set; }

        public IEnumerable<InventoryReceiptDetailDto> InventoryDetailDtos { get; set; }

    }
}
