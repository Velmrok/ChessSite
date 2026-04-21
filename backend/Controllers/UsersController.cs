using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Users;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IDistributedCache _cache;
        public UsersController(IUsersService usersService, IDistributedCache cache  )
        {
            _usersService = usersService;
            _cache = cache;
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {
            var cacheKey = $"users:{query.Page}:{query.Limit}:{query.Search}:{query.SortBy}:{query.SortDescending}:{query.RatingType}:{query.MinRating}:{query.MaxRating}:{query.Online}";

            var cached = await _cache.GetStringAsync(cacheKey);
            Response.Headers["X-Cache"] = cached != null ? "HIT" : "MISS";
            var usersResponse = await _usersService.GetAllUsersAsync(query);

            return Ok(usersResponse);
        }

    }
}