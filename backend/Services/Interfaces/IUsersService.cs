using backend.DTO.Users;
using backend.Services.Results;
using ErrorOr;


namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<ErrorOr<UsersResult>> GetAllUsersAsync(GetUsersQuery query);
        public Task<ErrorOr<UserProfileResponse>> GetUserProfileAsync(string nickname);
        public Task<ErrorOr<FriendsResponse>> GetFriendsAsync(string nickname, PaginationQuery pagination);
        public Task<ErrorOr<Success>> AddFriendAsync(string nickname, string currentUserNickname);
        public Task<ErrorOr<Success>> RemoveFriendAsync(string nickname, string currentUserNickname);
        public Task<ErrorOr<UpdateUserBioResponse>> UpdateUserBioAsync(string nickname, UpdateUserBioRequest request);
        public Task<ErrorOr<UpdateUserProfilePictureResponse>> UpdateUserProfilePictureAsync(string nickname, UpdateUserProfilePictureRequest request);
        public Task<ErrorOr<FriendsResponse>> GetOnlineFriendsAsync(string nickname, PaginationQuery pagination);
    }   
}