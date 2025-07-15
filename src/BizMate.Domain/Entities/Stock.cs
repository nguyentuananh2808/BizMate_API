namespace BizMate.Domain.Entities
{
    public class Stock : BaseEntity
    {

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; } 
    }
}
