namespace BizMate.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Người nhận thông báo (có thể null nếu là broadcast).
        /// </summary>
        public Guid StoreId { get; set; }
        public Guid? UserId { get; set; }

        /// <summary>
        /// Nếu thông báo liên quan đến một đơn hàng.
        /// </summary>
        public Guid? OrderId { get; set; }

        /// <summary>
        /// Nội dung thông báo hiển thị.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Loại thông báo (system, order, promotion...).
        /// </summary>
        public string Type { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
