using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Users;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _usersService.GetAllUsersAsync();
            var usersResponse = users.Select(u => new UserResponse
            (
                u.Nickname,
                u.Login,
                u.Email,
                u.ProfileBio,
                u.ProfilePictureUrl,
                u.CreatedAt,
                u.LastActive,
                u.RapidRating,
                u.BlitzRating,
                u.BulletRating,
                u.Role
            )).ToList();
            
            return Ok(usersResponse);
        }

    }
}