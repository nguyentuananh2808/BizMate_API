namespace BizMate.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public string PasswordSalt { get; set; } = default!;
        public string Role { get; set; } = "Owner"; // or "Staff"

        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;
        public ICollection<InventoryReceipt> CreatedReceipts { get; set; } = new List<InventoryReceipt>();
    }

}
