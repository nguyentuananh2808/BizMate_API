namespace BizMate.Domain.Entities
{
    public class OrderTechnician
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = default!;

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
