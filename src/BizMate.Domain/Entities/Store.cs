namespace BizMate.Domain.Entities
{
    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();
        public ICollection<InventoryReceipt> InventoryReceipts { get; set; } = new List<InventoryReceipt>();
    }
}
