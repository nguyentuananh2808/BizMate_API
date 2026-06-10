namespace BizMate.Domain.Entities
{
    public class Technician : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ZaloPhone { get; set; }

        public ICollection<TechnicianHolding> Holdings { get; set; } = new List<TechnicianHolding>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<OrderTechnician> OrderTechnicians { get; set; } = new List<OrderTechnician>();
    }
}
