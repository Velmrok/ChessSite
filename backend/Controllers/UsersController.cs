using backend.DTO.Users;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;


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
            
           
            var usersResult = await _usersService.GetAllUsersAsync(query);

             Response.Headers["X-Cache"] = usersResult.IsCached ? "HIT" : "MISS";

            return Ok(usersResult.Response);
        }

    }
}