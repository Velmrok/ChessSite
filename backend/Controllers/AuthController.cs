using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Errors;
using backend.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var token = result.Value.Token;
            CookieService.SetJwtCookie(Response, token);
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
            var token = result.Value.Token;
            CookieService.SetJwtCookie(Response, token);
            return Ok();
        }
    }
}