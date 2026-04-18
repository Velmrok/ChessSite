using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Models;
using backend.Services.Results;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ErrorOr<AuthResult>> RegisterAsync(RegisterRequest request);
        public Task<ErrorOr<AuthResult>> LoginAsync(LoginRequest request);
        public Task<ErrorOr<Success>> LogoutAsync(string refreshToken);
        public Task<ErrorOr<AuthResult>> RefreshAsync(string refreshToken);
        public Task<ErrorOr<GetMeResponse>> GetMeAsync(string sub);
    }
}