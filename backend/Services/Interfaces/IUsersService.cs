using backend.DTO.Users;


namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<List<UserResponse>> GetAllUsersAsync(GetUsersQuery query);
    }
}