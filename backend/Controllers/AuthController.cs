using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Errors;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            var accessToken = result.Value.AccessToken;
            var refreshToken = result.Value.RefreshToken;
            CookieService.SetJwtCookie(Response, accessToken);
            CookieService.SetRefreshTokenCookie(Response, refreshToken!);
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request); 
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            var accessToken = result.Value.AccessToken;
            CookieService.SetJwtCookie(Response, accessToken);
            if (result.Value.RefreshToken != null)
            {
                var refreshToken = result.Value.RefreshToken;
                CookieService.SetRefreshTokenCookie(Response, refreshToken);
            }
           return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.RefreshAsync(refreshToken!);
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            var accessToken = result.Value.AccessToken;
            CookieService.SetJwtCookie(Response, accessToken);
    
            return StatusCode(StatusCodes.Status201Created);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.LogoutAsync(refreshToken!);
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}