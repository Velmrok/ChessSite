using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Auth;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<List<User>> RegisterAsync(RegisterRequest request);
    }
}