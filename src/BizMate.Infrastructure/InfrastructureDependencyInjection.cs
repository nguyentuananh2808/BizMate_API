using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Mappings;
using BizMate.Application.Common.Security;
using BizMate.Infrastructure.Persistence.Repositories;
using BizMate.Infrastructure.Persistence;
using BizMate.Infrastructure.Security;
using BizMate.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;
using System.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using SqlKata.Compilers;
using Microsoft.EntityFrameworkCore;

public static class InfrastructureDependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
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



        // AutoMapper
        services.AddAutoMapper(typeof(UserMappingProfile));
        services.AddAutoMapper(typeof(ProductMappingProfile));
        services.AddAutoMapper(typeof(ProductCategoryMappingProfile));
        services.AddAutoMapper(typeof(ExportReceiptMappingProfile));
        services.AddAutoMapper(typeof(ImportReceiptMappingProfile));
        services.AddAutoMapper(typeof(NotificationMappingProfile));

        // Security
        services.AddScoped<IJwtFactory, JwtFactory>();
        services.AddScoped<ITokenFactory, TokenFactory>();

        // Session, Email, OTP
        services.AddScoped<IUserSession, CurrentUserService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IOtpStore, OtpRedisService>();

        // HttpContext accessor
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IUserSession, CurrentUserService>();

        // Dapper - SqlKata
        services.AddScoped<IDbConnection>(sp =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connection = new NpgsqlConnection(config.GetConnectionString("DefaultConnection"));
            return connection;
        });


        services.AddScoped<QueryFactory>(sp =>
        {
            var connection = sp.GetRequiredService<IDbConnection>();
            var compiler = new PostgresCompiler();
            return new QueryFactory(connection, compiler);
        });

        return services;
    }
}
