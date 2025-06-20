namespace BizMate.Domain.Entities
{
    public class Lookup
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = default!; // ví dụ: "ReceiptType", "Unit"
        public string Key { get; set; } = default!;  // ví dụ: "Import", "kg"
        public string Value { get; set; } = default!; // ví dụ: "Nhập kho", "Kilogram"
    }

}
