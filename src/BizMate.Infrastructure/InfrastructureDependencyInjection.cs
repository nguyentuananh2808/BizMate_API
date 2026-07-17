// FILE: src/BizMate.Infrastructure/InfrastructureDependencyInjection.cs
// Chỉ thêm 2 dòng đăng ký repo mới vào đúng vị trí — giữ nguyên toàn bộ code cũ

using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Mappings;
using BizMate.Application.Common.Security;
using BizMate.Application.UserCases.InventoryChat;
using BizMate.Domain.Constants;
using BizMate.Infrastructure.Migrations;
using BizMate.Infrastructure.Persistence.Repositories;
using BizMate.Infrastructure.Security;
using BizMate.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── Repositories hiện có ──────────────────────────────────────────────
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        services.AddScoped<ICodeGeneratorRepository, CodeGeneratorRepository>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDealerLevelRepository, DealerLevelRepository>();
        services.AddScoped<IDealerPriceRepository, DealerPriceRepository>();
        services.AddScoped<IExportReceiptRepository, ExportReceiptRepository>();
        services.AddScoped<IImportReceiptRepository, ImportReceiptRepository>();
        services.AddScoped<IImportReceiptDetailRepository, ImportReceiptDetailRepository>();
        services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IStatusRepository, StatusRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryServiceRepository>();
        services.AddScoped<IExportRepository, ExportOrderRepository>();
        services.AddScoped<ITechnicianHoldingRepository, TechnicianHoldingRepository>();

        // ── Repositories — phân quyền (đã thêm trước)
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();

        // ── Repositories — Serial tracking (MỚI THÊM) ────────────────────────
        services.AddScoped<IProductItemRepository, ProductItemRepository>();
        services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
        services.AddScoped<IInventoryChatRepository, InventoryChatRepository>();
        services.AddScoped<IInventoryQuestionParser, InventoryQuestionParser>();

        // ── AutoMapper ────────────────────────────────────────────────────────
        services.AddAutoMapper(typeof(UserMappingProfile));
        services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddAutoMapper(typeof(ProductCategoryMappingProfile));
        services.AddAutoMapper(typeof(ExportReceiptMappingProfile));
        services.AddAutoMapper(typeof(ImportReceiptMappingProfile));
        services.AddAutoMapper(typeof(NotificationMappingProfile));

        // ── Security ──────────────────────────────────────────────────────────
        services.AddScoped<IJwtFactory, JwtFactory>();
        services.AddScoped<ITokenFactory, TokenFactory>();

        // ── Authorization Handler ─────────────────────────────────────────────
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // ── Session, Email, OTP ───────────────────────────────────────────────
        services.AddScoped<IUserSession, CurrentUserService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IOtpStore, OtpRedisService>();

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // ── Dapper - SqlKata ──────────────────────────────────────────────────
        services.AddScoped<IDbConnection>(sp =>
        {
            var config     = sp.GetRequiredService<IConfiguration>();
            var connection = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
            return connection;
        });

        services.AddScoped<QueryFactory>(sp =>
        {
            var connection = sp.GetRequiredService<IDbConnection>();
            var compiler   = new PostgresCompiler();
            return new QueryFactory(connection, compiler);
        });

        return services;
    }
}
