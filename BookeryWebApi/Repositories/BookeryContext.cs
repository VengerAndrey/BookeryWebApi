using Microsoft.EntityFrameworkCore;

namespace BookeryWebApi.Repositories
{
    public class BookeryContext : DbContext
    {
        public BookeryContext(DbContextOptions<BookeryContext> options) : base(options) { }
    }
}
