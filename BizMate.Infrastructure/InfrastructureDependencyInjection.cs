using BizMate.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using BizMate.Application.Common.Interfaces.Repositories;
using BizMate.Infrastructure.Persistence.Repositories;
using BizMate.Application.Common.Mappings;
using BizMate.Application.Common.Security;
using BizMate.Infrastructure.Security;
using BizMate.Public.Auth;
using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Infrastructure.Services;

namespace BizMate.Infrastructure
{
    public static class InfrastructureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddAutoMapper(typeof(UserMappingProfile));
            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddScoped<ITokenFactory, TokenFactory>();
            services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();
            services.AddScoped<IUserSession, UserSession>();
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IOtpStore, OtpRedisService>();

            return services;
        }
    }
}
