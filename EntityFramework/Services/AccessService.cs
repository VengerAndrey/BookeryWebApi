using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class AccessService : IAccessService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public AccessService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> AccessById(string email, Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user is null)
            {
                return false;
            }

            var share = await context.Shares.FirstOrDefaultAsync(x => x.Id == id);
            if (share is null)
            {
                return false;
            }

            if (user.Shares.FirstOrDefault(x => x.Id == id && x.OwnerId == user.Id) == null)
            {
                user.Shares.Add(share);
                await context.SaveChangesAsync();
            }

            return true;
        }
    }
}