using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BizMate.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options, IUserSession userSession) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Stock> Stocks { get; set; } = default!;
        public DbSet<InventoryReceipt> InventoryReceipts => Set<InventoryReceipt>();
        public DbSet<InventoryReceiptDetail> InventoryReceiptDetails => Set<InventoryReceiptDetail>();
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<CodeSequence> CodeSequences { get; set; }
    }
}
