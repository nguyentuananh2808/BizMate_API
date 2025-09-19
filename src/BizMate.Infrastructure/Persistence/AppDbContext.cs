using BizMate.Application.Common.Security;
using BizMate.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        #region DbSet
        public DbSet<User> Users => Set<User>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<ImportReceipt> ImportReceipts => Set<ImportReceipt>();
        public DbSet<ImportReceiptDetail> ImportReceiptDetails => Set<ImportReceiptDetail>();
        public DbSet<ExportReceipt> ExportReceipts => Set<ExportReceipt>();
        public DbSet<ExportReceiptDetail> ExportReceiptDetails => Set<ExportReceiptDetail>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<OtpVerification> OtpVerifications => Set<OtpVerification>();
        public DbSet<CodeSequence> CodeSequences => Set<CodeSequence>();
        public DbSet<DealerLevel> DealerLevels => Set<DealerLevel>();
        public DbSet<DealerPrice> DealerPrices => Set<DealerPrice>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<Notification> Notifications => Set<Notification>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Details)
                .WithOne(d => d.Order)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Restrict); // ← quan trọng: giữ Order khi xóa Detail
        }

    }
}
