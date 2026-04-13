using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.DTO.Auth;

namespace backend.Services.Interfaces
{
    public interface IAuthService
    {
        public Task RegisterAsync(RegisterRequest request);
    }
}