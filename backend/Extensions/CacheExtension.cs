

using StackExchange.Redis;

public static class CacheExtension
{
    public static IServiceCollection AddCustomCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
         var isTest = configuration.GetValue<bool>("IsTestEnvironment");
    
    if (isTest || string.IsNullOrWhiteSpace(redisConnectionString))
    {
        services.AddDistributedMemoryCache();
        return services;
    }
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = "ChessSite:";
        });
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(redisConnectionString));
        return services;
    }
}