namespace BizMate.Application.Common.Dto.CoreDto
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? OrderId { get; set; }
        public Guid StoreId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "system"; // phân loại
    }
}
