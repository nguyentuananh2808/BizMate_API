using BizMate.Api.UserCases.User.UserLogin;
using BizMate.Application.Common.Extensions;
using BizMate.Application.Common.Interfaces;
using BizMate.Application.Common.Message;
using BizMate.Application.Common.Requests.Validators;
using BizMate.Infrastructure.Persistence;
using BizMate.Infrastructure.Security;
using BizMate.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer(); // Cho Swagger hoạt động

        // Đăng ký Swagger
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BizMate API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Nhập token vào đây: Bearer {your token}"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        builder.Services.AddHttpClient<IImageUploader, ImageBBUploader>();
        builder.Services.AddScoped<IAppMessageService, CommonAppMessageUtils>();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.Configure<JwtOptions>(
            builder.Configuration.GetSection("JwtOptions"));

        var jwtOptions = builder.Configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis");
        });

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<SearchCoreValidator>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
            };
        });

        // Đăng ký các lớp trình xử lý đầu ra
        builder.Services.Scan(scan => scan
            .FromAssemblyOf<UserLoginPresenter>()
            .AddClasses(classes => classes.AssignableTo(typeof(IOutputPort<>)))
            .AsSelf()
            .WithScopedLifetime());

        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        });

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Cấu hình CORS: Cho phép mọi nguồn để client Angular gọi
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", policy =>
            {
                policy
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        // Kích hoạt Swagger ở mọi môi trường
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BizMate API v1");
        });

        app.UseCors("AllowAngular");

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        // Auto apply migration
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        app.Run();
    }
}
