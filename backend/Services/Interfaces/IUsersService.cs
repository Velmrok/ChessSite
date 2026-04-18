using backend.DTO.Users;


namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<UsersResponse> GetAllUsersAsync(GetUsersQuery query);
    }
}