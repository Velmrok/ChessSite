using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isTestEnvironment)
    {
        if (isTestEnvironment)
        {
            services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TestDatabase")));
            return services;
        }
        var host = configuration["ConnectionStrings:Host"];
        var db = configuration["ConnectionStrings:Database"];
        var user = configuration["ConnectionStrings:Username"];
        var password = File.ReadAllText("/run/secrets/db_password").Trim();
        var connectionString = $"Host={host};Database={db};Username={user};Password={password}";



        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString)
        );

        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return app;
        }
        db.Database.Migrate();

        return app;
    }
}