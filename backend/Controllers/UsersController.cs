using System.Reflection.Metadata;
using backend.DTO.Common;
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
    public class UsersController : MyControllerBase
    {
        private readonly IUsersService _usersService;
        public UsersController(IUsersService usersService, IDistributedCache cache  )
        {
            _usersService = usersService;
           
        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQuery query)
        {

           return  HandleError(await _usersService.GetAllUsersAsync(query), usersResult =>
             {
                 Response.Headers["X-Cache"] = usersResult.IsCached ? "HIT" : "MISS";
                 return Ok(usersResult.Response);
             });

        }
        [HttpGet("{nickname}/profile")]
        public async Task<IActionResult> GetUserProfile(string nickname)
        {
            return HandleError(await _usersService.GetUserProfileAsync(nickname), Ok);
           
        }

    }
}