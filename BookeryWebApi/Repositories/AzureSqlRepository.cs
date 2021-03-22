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
        private object _lock = new object();

        public AzureSqlRepository(BookeryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Container>> ListContainersAsync()
        {
            return await Task.Run(() => _context.Containers);
        }

        public async Task AddContainerAsync(Container container)
        {
            await Task.Run(() =>
            {
                lock (_lock)
                {
                    _context.Containers.Add(container);
                    _context.SaveChanges();
                }
            });
        }

        public async Task AddContainersAsync(List<Container> containers)
        {
            await Task.Run(() =>
            {
                _context.Containers.AddRange(containers);
                _context.SaveChanges();
            });
        }
    }
}
