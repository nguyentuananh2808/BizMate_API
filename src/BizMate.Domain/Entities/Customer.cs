namespace BizMate.Domain.Entities
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Address { get; set; }
        public Guid? DealerLevelId { get; set; }
    }

}
