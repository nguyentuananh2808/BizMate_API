using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class ImportReceiptCoreDto : BaseEntity
    {
        public string? SupplierName { get; set; }
        public string? DeliveryAddress { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public Guid? StatusId { get; set; }
        public string? StatusName { get; set; }
        public string? StatusCode { get; set; }
        public ICollection<ImportReceiptDetailDto> Details { get; set; } = new List<ImportReceiptDetailDto>();


    }
}
