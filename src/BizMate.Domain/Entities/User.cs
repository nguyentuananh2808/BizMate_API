namespace BizMate.Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string PasswordSalt { get; set; } = default!;
        public string Role { get; set; } = "Owner"; // or "Staff"
        public ICollection<InventoryReceipt> CreatedReceipts { get; set; } = new List<InventoryReceipt>();
    }

}
