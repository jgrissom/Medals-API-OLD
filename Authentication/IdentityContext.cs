using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore; 

public class IdentityContext : IdentityDbContext<ApplicationUser>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
}
