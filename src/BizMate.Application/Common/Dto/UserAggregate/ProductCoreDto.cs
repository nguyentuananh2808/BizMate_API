using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.UserAggregate
{
    public class ProductCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
        public int Quantity { get; set; }
        public int Unit { get; set; } = default!;
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
        public Guid? SupplierId { get; set; }
    }
}
