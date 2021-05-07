using System;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class ApiDbContextFactory
    {
        /*private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public ApiDbContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public ApiDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>();
            _configureDbContext(options);
            return new ApiDbContext(options.Options);
        }*/
    }
}
