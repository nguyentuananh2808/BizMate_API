using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BizMate.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        private readonly IUserSession _userSession;

        public AppDbContext(DbContextOptions<AppDbContext> options, IUserSession userSession)
            : base(options)
        {
            _userSession = userSession;
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<InventoryReceipt> InventoryReceipts => Set<InventoryReceipt>();
        public DbSet<InventoryReceiptDetail> InventoryReceiptDetails => Set<InventoryReceiptDetail>();
        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
        public DbSet<CodeSequence> CodeSequences => Set<CodeSequence>();
        public DbSet<DealerLevel> DealerLevels => Set<DealerLevel>();
        public DbSet<DealerPrice> DealerPrices => Set<DealerPrice>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<ProductCategory>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<InventoryReceipt>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<Stock>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<InventoryReceiptDetail>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<Customer>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

            modelBuilder.Entity<Supplier>()
                .Property(p => p.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

        }
    }
}
