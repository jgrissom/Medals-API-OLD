using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;

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

        public void PatchCountry(int id, JsonPatchDocument<Country> patch)
        {
            Country country = this.Countries.FirstOrDefault(c => c.Id == id);
            patch.ApplyTo(country);
            this.SaveChanges();
        }
    }
}
