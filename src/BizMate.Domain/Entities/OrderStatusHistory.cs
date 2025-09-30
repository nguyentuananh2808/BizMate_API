namespace BizMate.Domain.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = default!;

        public Guid OldStatusId { get; set; }
        public Guid NewStatusId { get; set; }

        public string? Note { get; set; }   // Ghi chú (nếu có)

        // Ai thay đổi
        public Guid ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; } = default!;

        // Thời gian thay đổi
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
