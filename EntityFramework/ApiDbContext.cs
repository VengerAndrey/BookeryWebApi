using System;
using System.Linq;
using Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class ApiDbContext : DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserNode> UserNodes { get; set; }
        public DbSet<AccessType> AccessTypes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "(localdb)\\MSSQLLocalDB",
                InitialCatalog = "BookeryDb",
                IntegratedSecurity = true
            };

            optionsBuilder.UseSqlServer(builder.ConnectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Node>()
                .HasKey(x => x.Id);
            modelBuilder
                .Entity<Node>()
                .Property(x => x.Name).IsRequired();
            modelBuilder
                .Entity<Node>()
                .Property(x => x.IsDirectory).IsRequired();
            modelBuilder
                .Entity<Node>()
                .Property(x => x.Size);
            modelBuilder
                .Entity<Node>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder
                .Entity<Node>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedNodes)
                .HasForeignKey(x => x.OwnerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<Node>()
                .HasOne(x => x.ModifiedBy)
                .WithMany(x => x.ModifiedNodes)
                .HasForeignKey(x => x.ModifiedById)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder
                .Entity<User>()
                .Property(x => x.Email).IsRequired();
            modelBuilder
                .Entity<User>()
                .Property(x => x.Password).IsRequired();
            modelBuilder
                .Entity<User>()
                .Property(x => x.LastName).IsRequired();
            modelBuilder
                .Entity<User>()
                .Property(x => x.FirstName).IsRequired();

            modelBuilder
                .Entity<UserNode>()
                .HasKey(x => new { x.UserId, x.NodeId });
            modelBuilder
                .Entity<UserNode>()
                .HasOne(x => x.User)
                .WithMany(x => x.UserNodes)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder
                .Entity<UserNode>()
                .Property(x => x.AccessTypeId)
                .HasConversion<int>()
                .IsRequired();

            modelBuilder
                .Entity<AccessType>()
                .Property(x => x.AccessTypeId)
                .HasConversion<int>()
                .IsRequired();
            modelBuilder
                .Entity<AccessType>()
                .Property(x => x.Name)
                .IsRequired();
            modelBuilder
                .Entity<AccessType>()
                .HasData(Enum.GetValues(typeof(AccessTypeId))
                    .Cast<AccessTypeId>()
                    .Select(x => new AccessType
                    {
                        AccessTypeId = x,
                        Name = x.ToString()
                    }));

            base.OnModelCreating(modelBuilder);
        }
    }
}