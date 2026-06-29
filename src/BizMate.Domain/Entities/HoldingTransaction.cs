namespace BizMate.Domain.Entities
{
    public class HoldingTransaction : Base
    {
        public Guid TechnicianId { get; set; }
        public Technician Technician { get; set; } = default!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public HoldingTransactionType Type { get; set; }
        public TechnicianBorrowType BorrowType { get; set; } = TechnicianBorrowType.Assigned;
        public int Quantity { get; set; }

        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? Note { get; set; }
    }

    public enum HoldingTransactionType
    {
        Borrow = 1,
        Return = 2,
        ConvertToSale = 3,
        ManualAdjustment = 4
    }
}
