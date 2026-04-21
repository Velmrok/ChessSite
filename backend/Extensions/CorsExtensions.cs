namespace backend.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        return services;
    }
    public static WebApplication UseCustomCors(this WebApplication app)
    {
        app.UseCors(policy =>
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
        );
        return app;
    }
}