using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class DealerLevelCoreDto : BaseEntity
    {
        public string Name { get; set; } = default!;
        public ICollection<DealerPrice> DealerPrices { get; set; } = new List<DealerPrice>();
    }

}
