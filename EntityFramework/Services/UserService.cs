using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public UserService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            var users = await context.Users
                .Include(x => x.Shares)
                .ToListAsync();

            return users;
        }

        public async Task<User> Get(int id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Shares)
                .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<User> Create(User entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            var createdResult = await context.Users.AddAsync(entity);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<User> Update(int id, User entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            entity.Id = id;
            context.Users.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var entity = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            context.Users.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<User> GetByEmail(string email)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Shares)
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<User> GetByUsername(string username)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Shares)
                .FirstOrDefaultAsync(x => x.Username == username);

            return user;
        }

        public async Task<bool> AddShare(int userId, Guid shareId)
        {
            await using var context = _contextFactory.CreateDbContext();

            return false;
        }
    }
}