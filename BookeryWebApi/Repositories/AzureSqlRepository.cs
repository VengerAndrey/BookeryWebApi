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
    }
}
