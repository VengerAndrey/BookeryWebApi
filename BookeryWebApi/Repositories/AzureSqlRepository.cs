using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Models;

namespace BookeryWebApi.Repositories
{
    public class AzureSqlRepository : IDataRepository
    {
        private readonly BookeryContext _context;
        private readonly object _lock = new object();

        public AzureSqlRepository(BookeryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            return await Task.Run(() => _context.Containers);
        }

        public async Task<Container> AddContainerAsync(Container container)
        {
            await _context.Containers.AddAsync(container);
            lock (_lock)
            {
                _context.SaveChanges();
            }
            return container;
        }
    }
}
