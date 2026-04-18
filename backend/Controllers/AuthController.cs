using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Errors;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace backend.Controllers
{
    [ApiController]
    [Route("/auth")]
    public class AuthController : ControllerBase
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
            var result = await _authService.RegisterAsync(request);
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            var accessToken = result.Value.AccessToken;
            var refreshToken = result.Value.RefreshToken;
            _cookieService.SetJwtCookie(Response, accessToken);
            _cookieService.SetRefreshTokenCookie(Response, refreshToken!);
            return Created($"/users/{result.Value.User.Nickname}/profile", result.Value.User);
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
            _cookieService.SetJwtCookie(Response, accessToken);
            if (result.Value.RefreshToken != null)
            {
                var refreshToken = result.Value.RefreshToken;
                _cookieService.SetRefreshTokenCookie(Response, refreshToken);
            }
           return Ok(result.Value.User);
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
            var newRefreshToken = result.Value.RefreshToken;

            _cookieService.SetJwtCookie(Response, accessToken);
            _cookieService.SetRefreshTokenCookie(Response, newRefreshToken);

            return Ok(result.Value.User);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var result = await _authService.LogoutAsync(refreshToken!);
            _cookieService.DeleteJwtCookie(Response);
            _cookieService.DeleteRefreshTokenCookie(Response);
            if (result.IsError)
            {
                var error = result.FirstError;
                
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            
            return NoContent();
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            var result = await _authService.GetMeAsync(sub!);
            if (result.IsError)
            {
                var error = result.FirstError;
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }

            return Ok(result.Value);
        }

        
    }



}
