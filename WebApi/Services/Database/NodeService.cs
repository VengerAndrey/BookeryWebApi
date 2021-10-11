using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Services.Database
{
    public class NodeService : INodeService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;

        public NodeService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Node>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            var items = await context.Nodes.ToListAsync();

            return items;
        }

        public async Task<Node> Get(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var item = await context.Nodes
                .FirstOrDefaultAsync(x => x.Id == id);

            return item;
        }

        public async Task<Node> Create(Node entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            entity.Id = new Guid();

            var createdResult = await context.Nodes.AddAsync(entity);
            await context.SaveChangesAsync();

            return createdResult.Entity;
        }

        public async Task<Node> Update(Node entity)
        {
            await using var context = _contextFactory.CreateDbContext();

            context.Nodes.Update(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var entity = await context.Nodes.FirstOrDefaultAsync(x => x.Id == id);
            context.Nodes.Remove(entity);
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
