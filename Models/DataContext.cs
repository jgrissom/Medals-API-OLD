using Microsoft.EntityFrameworkCore;

namespace Medals_API.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }
    }
}
