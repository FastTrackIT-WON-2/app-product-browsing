using Microsoft.EntityFrameworkCore;
using AppProductBrowsing.Data.Entities;

namespace AppProductBrowsing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext (DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }

        public DbSet<Product> Product { get; set; }
    }
}
