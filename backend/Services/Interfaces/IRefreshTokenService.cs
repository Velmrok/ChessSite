
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> CreateRefreshTokenAsync(User user);
        Task<bool> DoesRefreshTokenExistAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
       
    }
}