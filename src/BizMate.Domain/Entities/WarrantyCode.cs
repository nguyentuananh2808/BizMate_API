namespace BizMate.Domain.Entities
{
    public class WarrantyCode
    {
        public Guid Id { get; set; }
        public string WarrantyCodeValue { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public DateTime WarrantyStartDate { get; set; }
        public int WarrantyDays { get; set; }
        public DateTime WarrantyEndDate { get; set; }
        public int TotalWarrantyTimes { get; set; }
        public int UsedWarrantyTimes { get; set; }
        public bool isAvailable { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

}
