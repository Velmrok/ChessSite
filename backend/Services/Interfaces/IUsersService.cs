using backend.DTO.Users;
using backend.Services.Results;


namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<UsersResult> GetAllUsersAsync(GetUsersQuery query);
    }
}