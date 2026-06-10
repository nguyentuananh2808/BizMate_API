namespace BizMate.Domain.Entities
{
    public class TechnicianHolding : Base
    {
        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }
        public DateTime LastBorrowedAt { get; set; } = DateTime.UtcNow;
    }
}
