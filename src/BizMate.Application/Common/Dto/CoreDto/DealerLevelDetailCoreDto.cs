using BizMate.Domain.Entities;

namespace BizMate.Application.Common.Dto.CoreDto
{
    public class DealerLevelDetailCoreDto : Base
    {
        public string Name { get; set; } = default!;

        //get info dealer level
        public ICollection<DealerPriceForDealerLevelDetailCoreDto> DealerPriceForDealerLevel { get; set; } = new List<DealerPriceForDealerLevelDetailCoreDto>();
    }

    public class DealerPriceForDealerLevelDetailCoreDto
    {
        public Guid DealerPriceId { get; set; } 
        // get info product
        public Guid ProductId { get; set; }
        public string? NameProduct { get; set; } 
        public int UnitProduct { get; set; }
        //get info dealer price
        public decimal Price { get; set; } 
        public Guid RowVersionDealerPrice { get; set; } 
    }
}
