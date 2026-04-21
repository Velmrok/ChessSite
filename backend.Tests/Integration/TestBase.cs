
using System.Net;
using backend.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;

namespace backend.Tests.Integration;
public class TestBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly HttpClient _client;

    public TestBase(WebApplicationFactory<Program> factory)
    {
        
        var dbName = Guid.NewGuid().ToString();

        var customizedFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                 configBuilder.Sources.Clear();
                 
                var testConfig = new Dictionary<string, string>
                {
                    {"ConnectionStrings:Redis", ""},
                    {"IsTestEnvironment", "true"},
                    {"Jwt:Key", "SuperTajnyKluczTestowyKtoryMaOdpowiedniaDlugosc123!"},
                    {"Jwt:Issuer", "test_issuer"},
                    {"Jwt:Audience", "test_audience"},
                    {"Jwt:ExpiryMinutes", "60"}
                };
                configBuilder.AddInMemoryCollection(testConfig!);
            });
            builder.ConfigureServices(services =>
           {

               var dbContextDescriptors = services.Where(
                   d => d.ServiceType.Name.Contains("DbContextOptions") ||
                        d.ServiceType.Name.Contains("DbConnection")).ToList();

               foreach (var descriptor in dbContextDescriptors)
               {
                   services.Remove(descriptor);
               }

               var redisDescriptors = services.Where(
                  d => d.ServiceType == typeof(IDistributedCache) ||
                       d.ServiceType == typeof(IConnectionMultiplexer)).ToList();

               foreach (var descriptor in redisDescriptors)
               {
                   services.Remove(descriptor);
               }
                services.AddDbContext<AppDbContext>(options =>
               {
                   options.UseInMemoryDatabase(dbName);
               });

               services.AddDistributedMemoryCache();

              

               services.AddSingleton(sp => 
                {
                    var mockMultiplexer = Substitute.For<IConnectionMultiplexer>();
                    var mockServer = Substitute.For<IServer>();
                    var mockDatabase = Substitute.For<IDatabase>();
                    
                    mockMultiplexer.GetServer(Arg.Any<EndPoint>())
                        .Returns(mockServer);
                    mockMultiplexer.GetEndPoints(Arg.Any<bool>())
                        .Returns([new IPEndPoint(0, 0)]);
                    mockMultiplexer.GetDatabase(Arg.Any<int>(), Arg.Any<object>())
                        .Returns(mockDatabase);
                    
                    mockServer.Keys(Arg.Any<int>(), Arg.Any<RedisValue>(), Arg.Any<int>(), Arg.Any<long>(), Arg.Any<int>(), Arg.Any<CommandFlags>())
                        .Returns([]);
                    
                    return mockMultiplexer;
                });
                
               
           });
        });

   


        _client = customizedFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }
}