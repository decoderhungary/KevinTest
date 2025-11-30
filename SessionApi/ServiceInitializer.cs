using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repository.Core;
using Repository.Services;

namespace SessionApi;

public static class ServiceInitializer
{
    public static void RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddCoreDbContext();

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        services.AddMemoryCache();

        services.AddHealthChecks();

        services.AddHttpLogging();
        
        services.AddMediatR(cfg =>
        {
            cfg.RegisterGenericHandlers = true;
            cfg.Lifetime = ServiceLifetime.Scoped;
            cfg.AutoRegisterRequestProcessors = true;
            cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
        });
    }

    public static void InitializeDbContext(this IServiceProvider services)
    {
        var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        _ = context.Sessions.Any();
        scope.Dispose();
    }

    private static void AddCoreDbContext(this IServiceCollection services)
    {
        // IMPORTANT: Initialize SQLite provider
        SQLitePCL.Batteries.Init();

        // Create a single shared in-memory SQLite connection
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        services.AddSingleton(connection);

        services.AddDbContextPool<AppDbContext>((provider, options) =>
        {
            var conn = provider.GetRequiredService<SqliteConnection>();
            options.UseSqlite(conn);
        });

        services.AddDbServices();

        // services.AddHealthChecks()
        //     .AddDbContextCheck<AppDbContext>("SQLite", customTestQuery: async (context, token) =>
        //     {
        //         await context.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken: token);
        //         return true;
        //     });
    }

    private static void AddDbServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IServiceBase<>), typeof(ServiceBase<>));
    }
}