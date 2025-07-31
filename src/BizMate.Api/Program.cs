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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpClient<IImageUploader, ImageBBUploader>();

        builder.Services.AddScoped<IAppMessageService, CommonAppMessageUtils>();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        /*builder.Services.AddSingleton<IStringLocalizerFactory, ResourceManagerStringLocalizerFactory>();
        builder.Services.AddScoped(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));*/

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

        // Register validators
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

        // Cấu hình dịch vụ CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngular", builder =>
            {
                builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors("AllowAngular");
        }

        /* var supportedCultures = new[] { "en", "vi" };
         var localizationOptions = new RequestLocalizationOptions()
             .SetDefaultCulture("vi")
             .AddSupportedCultures(supportedCultures)
             .AddSupportedUICultures(supportedCultures);
         app.UseRequestLocalization(localizationOptions);*/

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        // TỰ ĐỘNG APPLY MIGRATION KHI KHỞI ĐỘNG
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }
        app.Run();
    }
}
