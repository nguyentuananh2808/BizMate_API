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

            // Cấu hình RowVersion tự động cho tất cả entity có thuộc tính RowVersion
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var rowVersionProperty = entityType
                    .GetProperties()
                    .FirstOrDefault(p => p.Name == "RowVersion" && p.ClrType == typeof(byte[]));

                if (rowVersionProperty != null)
                {
                    rowVersionProperty.IsConcurrencyToken = true;
                    rowVersionProperty.SetColumnType("bytea"); // PostgreSQL dùng bytea
                }
            }
        }

    }
}
