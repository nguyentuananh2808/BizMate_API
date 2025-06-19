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
            services.AddScoped<IUserSession, UserSession>();

            return services;
        }
    }
}
