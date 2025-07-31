namespace BizMate.Domain.Entities
{
    public class DealerLevel : BaseEntity
    {
        public string Name { get; set; } = default!; 
        public ICollection<DealerPrice> DealerPrices { get; set; } = new List<DealerPrice>();
    }

}
