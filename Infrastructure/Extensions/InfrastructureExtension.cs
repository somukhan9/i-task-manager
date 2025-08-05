using Application.Contracts;
using Domain.Entities;
using Infrastructure.Dapper;
using Infrastructure.Data;
using Infrastructure.Policies;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Extensions;

public static class InfrastructureExtension
{
    public static void AddSqlConnectionFactory(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(builder.Configuration));
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITaskService, TaskService>();
        builder.Services.AddScoped<IBackgroundService, BackgroundServicesImpl>();

        builder.Services.AddScoped<IAuthorizationHandler, TaskOwnerHandler>();
    }

    public static void AddTaskManagerDbContext(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<TaskManagerDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("TaskManagerDbConnectionString")));
    }

    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, Role>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedAccount = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<TaskManagerDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void AddApplicationCookie(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/login";
            options.LogoutPath = "/auth/logout";
            options.AccessDeniedPath = "/error/access-denied";
        });
    }

    public static void AddJwtBearer(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value!,
                    ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value!,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value!))
                };
            });
    }

    public static void AddBackgroundServiceFactoryManager(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped(typeof(IBackgroundServicesFactory<>), typeof(BackgroundServicesFactory<>));
    }

    public static async void UseDatabaseMigrateMiddleware(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            logger.LogError($"DB Migrate EF EXCEPTION :: {ex}");
            logger.LogError($"DB Migrate EF EXCEPTION MESSAGE :: {ex.Message}");
            throw;
        }
    }
}