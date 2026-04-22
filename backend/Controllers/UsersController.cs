using backend.DTO.Users;
using backend.Extensions;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;


namespace backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService, IDistributedCache cache  )
        {
            _usersService = usersService;
           
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {

            var result = await _usersService.GetAllUsersAsync(query);

            if(result.IsError)
            {
                var error = result.FirstError;
                
                return Problem(statusCode: error.ToStatusCode(), title: error.Code, detail: error.Description);
            }
            var usersResult = result.Value;

             Response.Headers["X-Cache"] = usersResult.IsCached ? "HIT" : "MISS";

            return Ok(usersResult.Response);
        }

    }
}