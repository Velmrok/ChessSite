using backend.DTO.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
namespace backend.Tests.Integration.Auth;


public class AuthEndpointTest : TestBase
{
    public AuthEndpointTest(WebApplicationFactory<Program> factory) : base(factory)
    {
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