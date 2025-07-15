namespace BizMate.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Address { get; set; }
    }

}
