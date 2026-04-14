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
        public Task<ErrorOr<Success>> RegisterAsync(RegisterRequest request);
    }
}