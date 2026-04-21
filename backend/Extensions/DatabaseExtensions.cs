using backend.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default"))
        );

        return services;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if(db.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return app; 
        }
        db.Database.Migrate();
        
        return app;
    }
}