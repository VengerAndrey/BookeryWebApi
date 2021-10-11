using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.Database
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

            var users = await context.Users.ToListAsync();

            return users;
        }

        public async Task<User> Get(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
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

        public async Task<User> Update(User entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            context.Users.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(Guid id)
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
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<User> GetByUsername(string username)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.FirstName == username);

            return user;
        }

        /*public async Task<bool> SharePath(Guid userId, string path)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user.SharedPaths.FirstOrDefault(x => x == path) == null)
            {
                user.SharedPaths.Add(path);
            }

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }*/
    }
}