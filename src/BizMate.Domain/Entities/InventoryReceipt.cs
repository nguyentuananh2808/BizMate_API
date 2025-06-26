namespace BizMate.Domain.Entities
{
    public class InventoryReceipt : BaseEntity
    {
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string InventoryCode { get; set; } = default!;

        /// <summary>
        /// "Import = 1" hoặc "Export = 2"
        /// </summary>
        public int Type { get; set; } = default!;

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = default!;

        // Thêm thông tin tĩnh tùy theo loại phiếu
        public string? SupplierName { get; set; }     // Cho phiếu nhập
        public string? CustomerName { get; set; }     // Cho phiếu xuất
        public string? CustomerPhone { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? Description { get; set; }

        // Tổng tiền của phiếu
        public decimal TotalAmount { get; set; }

        // Trạng thái thanh toán (cho phiếu xuất)
        public int? PaymentStatus { get; set; }

        // Trạng thái phiếu
        public bool IsDraft { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public ICollection<InventoryReceiptDetail> Details { get; set; } = new List<InventoryReceiptDetail>();
    }
}