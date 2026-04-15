
using System.Net;
using backend.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
                var testConfig = new Dictionary<string, string>
                {
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


               services.AddDbContext<AppDbContext>(options =>
               {
                   options.UseInMemoryDatabase(dbName);
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