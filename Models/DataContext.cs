using Microsoft.EntityFrameworkCore;

namespace Medals_API.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }

        public Country AddCountry(Country country)
        {
            this.Add(country);
            this.SaveChanges();
            return country;
        }

        public void DeleteCountry(Country country)
        {
            this.Remove(country);
            this.SaveChanges();
        }
    }
}
