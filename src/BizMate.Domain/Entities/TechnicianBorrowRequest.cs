namespace BizMate.Domain.Entities
{
    public class TechnicianBorrowRequest : BaseEntity
    {
        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = default!;

        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Daily;
        public TechnicianBorrowRequestStatus RequestStatus { get; set; } = TechnicianBorrowRequestStatus.Pending;

        public DateOnly NeededDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
        public DateTime? ApprovedAt { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? RejectionReason { get; set; }

        public ICollection<TechnicianBorrowRequestItem> Items { get; set; } = new List<TechnicianBorrowRequestItem>();
    }

    public class TechnicianBorrowRequestItem : BaseCoreEntity
    {
        public Guid TechnicianBorrowRequestId { get; set; }
        public TechnicianBorrowRequest TechnicianBorrowRequest { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string ProductName { get; set; } = default!;
        public string? ProductCode { get; set; }
        public int Quantity { get; set; }
    }

    public enum TechnicianBorrowRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Cancelled = 4
    }
}
