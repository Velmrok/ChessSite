using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Models;

namespace backend.Services.Interfaces
{
    public interface IUsersService
    {
        public Task<List<User>> GetAllUsersAsync();
    }
}