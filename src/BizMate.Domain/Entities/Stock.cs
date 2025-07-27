namespace BizMate.Domain.Entities
{
    public class Stock : Base
    {

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public int Quantity { get; set; } 
    }
}
