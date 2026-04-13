using backend.Data;
using backend.DTO.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
namespace backend.Tests.Integration;


public class AuthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

     public AuthEndpointTests(WebApplicationFactory<Program> factory)
    {
        
        var customizedFactory = factory.WithWebHostBuilder(builder =>
        {
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
    public async Task RegisterEndpoint_ShouldReturnOk_WhenUserIsNew()
    {
       
        var request = new RegisterRequest
        {
            Email = "nowy@test.com",
            Login = "nowyUser",
            Nickname = "nowyKozak",
            Password = "123456"
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);
        
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }
}