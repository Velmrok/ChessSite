using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Models;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ErrorOr<AuthResponse>> RegisterAsync(RegisterRequest request);
        public Task<ErrorOr<AuthResponse>> LoginAsync(LoginRequest request);
        public Task<ErrorOr<Success>> LogoutAsync(string refreshToken);
        public Task<ErrorOr<AuthResponse>> RefreshAsync(string refreshToken);
        public Task<ErrorOr<GetMeResponse>> GetMeAsync(string nickname);
    }
}