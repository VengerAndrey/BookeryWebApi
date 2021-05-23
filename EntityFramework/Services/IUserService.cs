﻿using System;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;

namespace EntityFramework.Services
{
    public interface IUserService : IDataService<User>
    {
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
        Task<bool> AddShare(int userId, Guid shareId);
    }
}