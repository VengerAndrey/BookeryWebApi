using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Services;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework.Services
{
    public class NodeService : INodeService
    {
        private readonly IDbContextFactory<ApiDbContext> _contextFactory;
        private readonly NonQueryDataService<Node> _nonQueryDataService;

        public NodeService(IDbContextFactory<ApiDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _nonQueryDataService = new NonQueryDataService<Node>(contextFactory);
        }

        public async Task<IEnumerable<Node>> GetAll()
        {
            await using var context = _contextFactory.CreateDbContext();

            var nodes = await context.Nodes
                .Include(x => x.Parent)
                //.Include(x => x.Owner)
                .ToListAsync();

            return nodes;
        }

        public async Task<Node> Get(int id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var node = await context.Nodes
                .Include(x => x.Parent)
                //.Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);

            return node;
        }

        public async Task<Node> Create(Node entity)
        {
            return await _nonQueryDataService.Create(entity);
        }

        public async Task<Node> Update(int id, Node entity)
        {
            return await _nonQueryDataService.Update(id, entity);
        }

        public async Task<bool> Delete(int id)
        {
            return await _nonQueryDataService.Delete(id);
        }

        public async Task<Node> GetWithSub(int id)
        {
            await using var context = _contextFactory.CreateDbContext();

            var node = await context.Nodes
                .Include(x => x.Parent)
                //.Include(x => x.Owner)
                .Include(x => x.SubNodes)
                .FirstOrDefaultAsync(x => x.Id == id);

            return node;
        }
    }
}
