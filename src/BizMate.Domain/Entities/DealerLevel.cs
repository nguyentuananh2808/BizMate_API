namespace BizMate.Domain.Entities
{
    public class DealerLevel : Base
    {
        public string Name { get; set; } = default!;
        public ICollection<DealerPrice> DealerPrices { get; set; } = new List<DealerPrice>();
    }

}
