// FILE: src/BizMate.Infrastructure/Persistence/AppDbContext.cs
// Chỉ thêm các phần mới — giữ nguyên toàn bộ code cũ

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

        #region DbSet — hiện có (giữ nguyên)
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
        public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
        public DbSet<WarrantyCode> WarrantyCodes => Set<WarrantyCode>();
        #endregion

        #region DbSet — phân quyền (đã thêm trước)
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        #endregion

        #region DbSet — Serial tracking (MỚI THÊM)
        public DbSet<ProductItem> ProductItems => Set<ProductItem>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Cấu hình cũ giữ nguyên ───────────────────────────────────────
            modelBuilder.Entity<WarrantyCode>()
                .HasIndex(x => x.WarrantyCodeValue)
                .IsUnique();

            modelBuilder.Entity<Permission>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
                b.Property(x => x.Group).IsRequired().HasMaxLength(50);
                b.HasIndex(x => x.Name).IsUnique();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasKey(x => x.Id);
                b.Property(x => x.Name).IsRequired().HasMaxLength(100);
                b.Property(x => x.DisplayName).IsRequired().HasMaxLength(200);
                b.HasMany(x => x.RolePermissions).WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
                b.HasMany(x => x.UserRoles).WithOne(x => x.Role)
                    .HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RolePermission>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.RoleId, x.PermissionId }).IsUnique();
                b.HasOne(x => x.Permission).WithMany(x => x.RolePermissions)
                    .HasForeignKey(x => x.PermissionId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserRole>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.UserId, x.RoleId, x.StoreId }).IsUnique();
                b.HasOne(x => x.User).WithMany(x => x.UserRoles)
                    .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne(x => x.Store).WithMany()
                    .HasForeignKey(x => x.StoreId).OnDelete(DeleteBehavior.Cascade);
            });

            // ── ProductItem (MỚI) ─────────────────────────────────────────────
            modelBuilder.Entity<ProductItem>(b =>
            {
                b.HasKey(x => x.Id);

                // UNIQUE constraint cho SerialNumber — chống duplicate dù có race condition
                b.HasIndex(x => x.SerialNumber)
                    .IsUnique()
                    .HasDatabaseName("IX_ProductItems_SerialNumber");

                // Index phụ để query nhanh
                b.HasIndex(x => new { x.StoreId, x.ProductId, x.Status })
                    .HasDatabaseName("IX_ProductItems_Store_Product_Status");

                b.Property(x => x.SerialNumber)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(x => x.Status)
                    .HasConversion<int>(); // lưu enum dạng int

                // FK → Product
                b.HasOne(x => x.Product)
                    .WithMany(x => x.ProductItems)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // không xóa Product nếu còn item

                // FK → ImportReceiptDetail (optional)
                b.HasOne(x => x.ImportReceiptDetail)
                    .WithMany(x => x.ProductItems)
                    .HasForeignKey(x => x.ImportReceiptDetailId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                // FK → OrderDetail (optional)
                b.HasOne(x => x.OrderDetail)
                    .WithMany(x => x.ProductItems)
                    .HasForeignKey(x => x.OrderDetailId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.SetNull);

                // FK → Store (kế thừa từ Base)
                b.HasOne(x => x.Store)
                    .WithMany()
                    .HasForeignKey(x => x.StoreId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ── InventoryTransaction (MỚI) ────────────────────────────────────
            modelBuilder.Entity<InventoryTransaction>(b =>
            {
                b.HasKey(x => x.Id);

                b.HasIndex(x => x.ProductItemId)
                    .HasDatabaseName("IX_InventoryTransactions_ProductItemId");

                b.Property(x => x.Type)
                    .HasConversion<int>();

                b.Property(x => x.FromStatus)
                    .HasConversion<int?>()
                    .IsRequired(false);

                b.Property(x => x.ToStatus)
                    .HasConversion<int>();

                b.Property(x => x.Note)
                    .HasMaxLength(500)
                    .IsRequired(false);

                // FK → ProductItem
                b.HasOne(x => x.ProductItem)
                    .WithMany(x => x.Transactions)
                    .HasForeignKey(x => x.ProductItemId)
                    .OnDelete(DeleteBehavior.Cascade);

                // InventoryTransaction KHÔNG có IsDeleted / soft-delete
                // → audit trail bất biến, không bao giờ xóa
            });
        }
    }
}
