using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class CustomerCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Address { get; set; } = default!;
        public Guid? DealerLevelId { get; set; }
        public DealerLevel? DealerLevel { get; set; }
    }
}
