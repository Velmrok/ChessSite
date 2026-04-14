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
        var jwtCookie = cookies.First(c => c.StartsWith("jwt="));

        var token = jwtCookie.Split(';')[0].Split('=')[1];

        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        

        
        jwt.Claims.Should().Contain(c => c.Type == "email" && c.Value == "nowy@test.com");
    }
}