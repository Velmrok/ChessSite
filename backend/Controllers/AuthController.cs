using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Extensions;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;

namespace backend.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : MyControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICookieService _cookieService;
        public AuthController(IAuthService authService, ICookieService cookieService)
        {
            _authService = authService;
            _cookieService = cookieService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            return HandleError(await _authService.RegisterAsync(request), authResult =>
             {
                 var accessToken = authResult.AccessToken;
                 var refreshToken = authResult.RefreshToken;
                 _cookieService.SetJwtCookie(Response, accessToken);
                 _cookieService.SetRefreshTokenCookie(Response, refreshToken!);
                 return Created($"/users/{authResult.User.Nickname}/profile", authResult.User);
             });

        }
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            return HandleError(await _authService.LoginAsync(request), LoginResult =>
             {
                 var accessToken = LoginResult.AccessToken;
                 _cookieService.SetJwtCookie(Response, accessToken);
                 if (LoginResult.RefreshToken != null)
                 {
                     var refreshToken = LoginResult.RefreshToken;
                     _cookieService.SetRefreshTokenCookie(Response, refreshToken);
                 }
                 return Ok(LoginResult.User);
             });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            return HandleError(await _authService.RefreshAsync(refreshToken!), result =>
             {
                 var accessToken = result.AccessToken;
                 var newRefreshToken = result.RefreshToken;

                 _cookieService.SetJwtCookie(Response, accessToken);
                 _cookieService.SetRefreshTokenCookie(Response, newRefreshToken);


                 return Ok(result.User);
             });

        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            _cookieService.DeleteJwtCookie(Response);
            _cookieService.DeleteRefreshTokenCookie(Response);
            return HandleError(await _authService.LogoutAsync(refreshToken!), _ =>
              {
                  return NoContent();
              });
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return HandleError(await _authService.GetMeAsync(sub!), Ok);
        }


    }



}
