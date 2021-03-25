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
