using backend.DTO.Users;
using backend.Services.Results;
using ErrorOr;


namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<ErrorOr<UsersResult>> GetAllUsersAsync(GetUsersQuery query);
    }
}