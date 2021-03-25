using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookeryWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookeryWebApi.Repositories
{
    public class BookeryContext : DbContext
    {
        public BookeryContext(DbContextOptions<BookeryContext> options) : base(options) { }
    }
}
