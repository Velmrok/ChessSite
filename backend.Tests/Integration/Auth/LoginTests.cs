using backend.DTO.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
namespace backend.Tests.Integration.Auth;


public class LoginTests : TestBase
{
    public LoginTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    [Fact]
    public async Task LoginEndpoint_ShouldReturnOk_WhenCredentialsAreValid()
    {
       await MakeUserAsync("test@test.com", "testUser", "testKozak", "123456");


        var request = new LoginRequest
        {
            Login = "testUser",
            Password = "123456"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var cookies = response.Headers.GetValues("Set-Cookie");
        var refreshCookie = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
        refreshCookie.Should().NotBeNull();
        var accessCookie = cookies.First(c => c.StartsWith("accessToken="));
        var accessToken = accessCookie.Split(';')[0].Split('=')[1];
        accessCookie.Should().NotBeNull();
    }
    
    [Fact]
    public async Task LoginEndpoint_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
    {
        
        var request = new LoginRequest
        {
            Login = "nonexistentUser",
            Password = "wrongPassword"
        };

        var response = await _client.PostAsJsonAsync("/auth/login", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
    [Fact]
    public async Task LoginEndpoint_ShouldReturn429TooManyRequests_WhenRateLimitExceeded()
    {
        var loginRequest = new LoginRequest
        {
            Login = "testUser",
            Password = "123456"
        };
        var response = await _client.PostAsJsonAsync("/auth/login", loginRequest);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        for (int i = 0; i < 10; i++)
        {
            await _client.PostAsJsonAsync("/auth/login", loginRequest);
            
        }

        var rateLimitedResponse = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        rateLimitedResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.TooManyRequests);
    }
}