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


public class AuthEndpointTests : TestBase
{
    
    public AuthEndpointTests(WebApplicationFactory<Program> factory) : base(factory)
    {
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
        var accessCookie = cookies.First(c => c.StartsWith("accessToken="));

        var accessToken = accessCookie.Split(';')[0].Split('=')[1];

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);

        var refreshCookie = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
        refreshCookie.Should().NotBeNull();
        

        
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "nowy@test.com");
    }
    [Fact]
    public async Task ShouldRegisterThenLoginThenLogoutSuccessfully()
    {
        var registerRequest = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "testUser",
            Nickname = "testKozak",
            Password = "123456"
        };
        var response1 = await _client.PostAsJsonAsync("/auth/register", registerRequest);
        response1.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var loginRequest = new LoginRequest
        {
            Login = "testUser",
            Password = "123456"
        };
        var response2 = await _client.PostAsJsonAsync("/auth/login", loginRequest);
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        var cookies = response2.Headers.GetValues("Set-Cookie");
        var refreshCookie = cookies.FirstOrDefault(c => c.StartsWith("refreshToken="));
        refreshCookie.Should().NotBeNull();
        var accessCookie = cookies.First(c => c.StartsWith("accessToken="));
        var accessToken = accessCookie.Split(';')[0].Split('=')[1];
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(accessToken);
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "test@test.com");

        var response3 = await _client.PostAsync("/auth/logout", null);
        response3.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        var response4 = await _client.PostAsync("/auth/refresh", null);
        response4.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }
}

