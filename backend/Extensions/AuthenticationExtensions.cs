using Microsoft.AspNetCore.Authentication.Cookies;

namespace backend.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddAuthenticationConfig(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Events.OnRedirectToLogin = ctx => { ctx.Response.StatusCode = 401; return Task.CompletedTask; };
            options.Events.OnRedirectToAccessDenied = ctx => { ctx.Response.StatusCode = 403; return Task.CompletedTask; };
        })
        .AddOpenIdConnect("oidc", options =>
        {
            options.Authority = "http://keycloak:8080/realms/chess";
            options.RequireHttpsMetadata = false;
            options.ClientId = "chess-frontend";
            options.CallbackPath = "/api/signin-oidc";
            options.ResponseMode = "query";
            options.NonceCookie.SameSite = SameSiteMode.Lax;
            options.CorrelationCookie.SameSite = SameSiteMode.Lax;
            options.SignedOutCallbackPath = "/api/signout-oidc";
            options.ClientSecret = configuration["Keycloak:FrontendSecret"];
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.Scope.Add("openid"); options.Scope.Add("profile"); options.Scope.Add("email");
            options.Scope.Add("offline_access");
            options.MapInboundClaims = false; 
            options.TokenValidationParameters.NameClaimType = "preferred_username";
        });
        services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) => options.SessionStore = store);
        return services;
    }
}