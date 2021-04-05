using BookeryWebApi.Entities;
using BookeryWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookeryWebApi.Common
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<ContainerEntity> Containers { get; set; }
        public DbSet<BlobEntity> Blobs { get; set; }
        public DbSet<UserEntity> Users { get; set; }
    }
}
