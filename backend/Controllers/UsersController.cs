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
        [HttpGet("{nickname}/friends")]
        public async Task<IActionResult> GetFriendsProfile(string nickname, [FromQuery] PaginationQuery pagination)
        {
            return HandleError(await _usersService.GetFriendsProfileAsync(nickname, pagination), Ok);
        }
        [HttpPost("{nickname}/friend")]
        public async Task<IActionResult> AddFriend(string nickname)
        {
            var currentUserNickname = User.FindFirst("nickname")?.Value;
            return HandleError(await _usersService.AddFriendAsync(nickname, currentUserNickname!), _=> NoContent());
        }
        [HttpDelete("{nickname}/friend")]
        public async Task<IActionResult> RemoveFriend(string nickname)
        {
            var currentUserNickname = User.FindFirst("nickname")?.Value;
            return HandleError(await _usersService.RemoveFriendAsync(nickname, currentUserNickname!), _=> NoContent());
        }
        [HttpPatch("me/profile/bio")]
        public async Task<IActionResult> UpdateUserBio([FromBody] UpdateUserBioRequest request)
        {
            var currentUserNickname = User.FindFirst("nickname")?.Value;
            return HandleError(await _usersService.UpdateUserBioAsync(currentUserNickname!, request), Ok);
        }
        [HttpPatch("me/profile/picture")]
        public async Task<IActionResult> UploadUserProfilePicture([FromForm] UpdateUserProfilePictureRequest request)
        {
            var currentUserNickname = User.FindFirst("nickname")?.Value;
            return HandleError(await _usersService.UpdateUserProfilePictureAsync(currentUserNickname!, request), Ok);
        }
        [HttpGet("me/friends-online")]
        public async Task<IActionResult> GetOnlineFriends([FromQuery] PaginationQuery pagination)
        {
            var currentUserNickname = User.FindFirst("nickname")?.Value;
            return HandleError(await _usersService.GetOnlineFriendsAsync(currentUserNickname!, pagination), Ok);
        }
    }
}