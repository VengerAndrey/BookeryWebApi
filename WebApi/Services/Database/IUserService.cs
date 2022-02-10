using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;

namespace WebApi.Services.Database
{
    public interface IUserService : IGuidDataService<User>
    {
        Task<User> GetByEmail(string email);
    }
}