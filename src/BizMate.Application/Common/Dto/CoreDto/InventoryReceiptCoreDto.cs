using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class InventoryReceiptCoreDto : BaseEntity
    {
        public string InventoryCode { get; set; } = default!;
        public DateTime Date { get; set; }
        public int Type { get; set; }

        public Guid StoreId { get; set; }
        public string StoreName { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = default!;

        public string? SupplierName { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }

        public string? Description { get; set; }
        public IEnumerable<InventoryReceiptDetailDto> InventoryDetailDtos { get; set; }

    }
}
