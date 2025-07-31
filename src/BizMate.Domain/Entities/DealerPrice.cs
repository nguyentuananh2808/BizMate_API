namespace BizMate.Domain.Entities
{
    public class DealerPrice : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public Guid DealerLevelId { get; set; }
        public DealerLevel DealerLevel { get; set; } = default!;

        public decimal Price { get; set; }
    }

}
