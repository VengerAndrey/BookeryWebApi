using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class ShareService : IShareService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public ShareService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Share>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            return context.Shares;
        }

        public async Task<Share> Get(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            return await context.Shares.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Share> Create(Share entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            var createdResult = await context.Shares.AddAsync(entity);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<Share> Update(Share entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            context.Shares.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var entity = await context.Shares.FirstOrDefaultAsync(x => x.Id == id);
            context.Shares.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
