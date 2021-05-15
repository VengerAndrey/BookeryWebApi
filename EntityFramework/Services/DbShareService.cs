using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class DbShareService : IDbShareService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public DbShareService(IDbContextFactory<ApiDbContext> contextFactory)
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
            var owner = await context.Users.FirstOrDefaultAsync(x => x.Id == entity.OwnerId);
            owner.Shares.Add(entity);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<Share> Update(Guid id, Share entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            entity.Id = id;
            context.Shares.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var entity = await context.Shares.Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.Id == id);
            context.Shares.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }
    }
}