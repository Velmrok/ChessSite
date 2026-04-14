using backend.Data;
using backend.DTO.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
namespace backend.Tests.Integration;


public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

     public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {

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
                   options.UseInMemoryDatabase("TestyIntegracyjneDb");
               });
           });
        });


        _client = customizedFactory.CreateClient();
    }

    [Fact]
    public async Task RegisterEndpoint_ShouldReturnCreated_WhenUserIsNew()
    {

        var request = new RegisterRequest
        {
            Email = "nowy@test.com",
            Login = "nowyUser",
            Nickname = "nowyKozak",
            Password = "123456"
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var cookies = response.Headers.GetValues("Set-Cookie");
        var jwtCookie = cookies.First(c => c.StartsWith("jwt="));

        var token = jwtCookie.Split(';')[0].Split('=')[1];

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        

        
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "nowy@test.com");
    }
}