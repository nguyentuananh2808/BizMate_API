using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<InventoryReceipt> InventoryReceipts => Set<InventoryReceipt>();
        public DbSet<InventoryReceiptDetail> InventoryReceiptDetails => Set<InventoryReceiptDetail>();
        public DbSet<Lookup> Lookups => Set<Lookup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed lookup 
            modelBuilder.Entity<Lookup>().HasData(
                new Lookup { Id = new Guid("17c94c00-f803-4497-a4d8-b3baf6d9457d"), Type = "ReceiptType", Key = "Import", Value = "Nhập kho" },
                new Lookup { Id = new Guid("d2aa8893-205d-42fa-9da7-02128bd72cd6"), Type = "ReceiptType", Key = "Export", Value = "Xuất kho" },
                new Lookup { Id = new Guid("379e938a-5bd6-4926-8748-f1403888185c"), Type = "Unit", Key = "kg", Value = "Kilogram" },
                new Lookup { Id = new Guid("2b812291-1133-4202-aea3-6c615d999b53"), Type = "Unit", Key = "pcs", Value = "Cái" },
                new Lookup { Id = new Guid("8b29b47b-aafc-490d-ba64-357ef9f0b4a8"), Type = "Unit", Key = "box", Value = "Thùng" },
                new Lookup { Id = new Guid("62298358-d4bd-4d86-911f-42eac3420135"), Type = "Unit", Key = "bag", Value = "Bao" }
            );
        }
    }
}
