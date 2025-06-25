using BizMate.Domain.Entities;

public class InventoryReceiptDetail : BaseEntity
{
    public Guid InventoryReceiptId { get; set; }
    public InventoryReceipt InventoryReceipt { get; set; } = default!;

    // Lưu thông tin sản phẩm tại thời điểm tạo phiếu (snapshot)
    public Guid ProductId { get; set; }           // để tham chiếu nếu cần
    public string ProductName { get; set; } = default!;
    public string? ProductCode { get; set; }      // nếu có
    public int Unit { get; set; }             // ví dụ: chiếc, hộp...

    public int Quantity { get; set; }
}
