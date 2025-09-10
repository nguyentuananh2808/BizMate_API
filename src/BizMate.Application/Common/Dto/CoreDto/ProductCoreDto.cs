using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.UserAggregate
{
    public class ProductCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int Available { get; set; }
        public int Unit { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public Guid? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public Guid ProductCategoryId { get; set; } 
        public string? ProductCategoryName { get; set; } 
    }
}
