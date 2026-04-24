using backend.DTO.Auth;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
namespace backend.Tests.Integration.Auth;


public class RegisterTests : TestBase
{
    public RegisterTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }
    [Fact]
    public async Task ShouldReturnCreated_WhenUserIsNew()
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
    [Theory]
    [InlineData("test@test.com", "differentUser", "differentNickname", "123456")]
    [InlineData("different@test.com", "testUser", "differentNickname", "123456")]
    [InlineData("different@test.com", "differentUser", "testKozak", "123456")]

    public async Task ShouldReturnBadRequest_WhenCredentialsAreTaken(string email, string login, string nickname, string password)
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "testUser",
            Nickname = "testKozak",
            Password = "123456"
        };

        var response = await _client.PostAsJsonAsync("/auth/register", request);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var secondRequest = new RegisterRequest
        {
            Email = email,
            Login = login,
            Nickname = nickname,
            Password = password
        };
        var secondResponse = await _client.PostAsJsonAsync("/auth/register", secondRequest);
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData(null, "testUser", "testKozak", "123456")]
    [InlineData("test@test.com", null, "testKozak", "123456")]
    [InlineData("test@test.com", "testUser", null, "123456")]
    [InlineData("test@test.com", "testUser", "testKozak", null)]
    public async Task ShouldReturnBadRequest_WhenCredentialsAreNull(string? email, string? login, string? nickname, string? password)
    {
        var request = new RegisterRequest
        {
            Email = email!,
            Login = login!,
            Nickname = nickname!,
            Password = password!
        };
        var response = await _client.PostAsJsonAsync("/auth/register", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task ShouldReturnCreated_WhenPassowordsAreSame()
    {
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Login = "testUser",
            Nickname = "testKozak",
            Password = "123456"
        };
        var response = await _client.PostAsJsonAsync("/auth/register", request);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var secondRequest = new RegisterRequest
        {
            Email = "different@test.com",
            Login = "differentUser",
            Nickname = "differentNickname",
            Password = "123456"
        };
        var secondResponse = await _client.PostAsJsonAsync("/auth/register", secondRequest);
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

    }
}