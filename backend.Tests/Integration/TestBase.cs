
using System.Net;
using System.Net.Http.Json;
using backend.Data;
using backend.DTO.Auth;
using backend.Models;
using Microsoft.AspNetCore.Identity;
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

    protected readonly AppDbContext _dbContext;
    protected readonly IDistributedCache _cache;
    
    private readonly IPasswordHasher<User> _passwordHasher;
    public TestBase(WebApplicationFactory<Program> factory)
    {

        var dbName = Guid.NewGuid().ToString();
        _passwordHasher = new PasswordHasher<User>();
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

         var scope = customizedFactory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();



        _client = customizedFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true
        });
    }

    protected async Task<User> MakeUserAsync(string email, string login, string nickname, string password)
    {
        var user = new User
        {
            Email = email,
            Login = login,
            Nickname = nickname,
            PasswordHash = _passwordHasher.HashPassword(null!, password)
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }
    protected async Task RegisterUserAsync(string email, string login, string nickname, string password)
    {
        var registerRequest = new RegisterRequest
        {
            Email = email,
            Login = login,
            Nickname = nickname,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/auth/register", registerRequest);
        response.EnsureSuccessStatusCode();
    }
    protected async Task LoginAsUserAsync(
        string email = "test@test.com",
        string login = "testuser",
        string nickname = "TestNick",
        string password = "123456")
    {

        await MakeUserAsync(email, login, nickname, password);

        var loginRequest = new LoginRequest
        {
            Login = login,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        
    }
}