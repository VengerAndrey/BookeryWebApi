using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class UserService : IUserService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly NonQueryDataService<User> _nonQueryDataService;

        public UserService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<User>(contextFactory);
        }
        public async Task<IEnumerable<User>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            var users = await context.Users
                .Include(x => x.Nodes)
                .ToListAsync();

            return users;
        }

        public async Task<User> Get(int id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Nodes)
                .FirstOrDefaultAsync(x => x.Id == id);

            return user;
        }

        public async Task<User> Create(User entity)
        {
            return await _nonQueryDataService.Create(entity);
        }

        public async Task<User> Update(int id, User entity)
        {
            return await _nonQueryDataService.Update(id, entity);
        }

        public async Task<bool> Delete(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<User> GetByEmail(string email)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Nodes)
                .FirstOrDefaultAsync(x => x.Email == email);

            return user;
        }

        public async Task<User> GetByUsername(string username)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users
                .Include(x => x.Nodes)
                .FirstOrDefaultAsync(x => x.Username == username);

            return user;
        }
    }
}
