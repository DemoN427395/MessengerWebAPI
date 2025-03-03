using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MessengerLib.Models;

// Base DbContext class with Identity support
public class BaseDbContext : IdentityDbContext<ApplicationUser>
{
    public BaseDbContext(DbContextOptions options) : base(options) { }

    // Default model creation for Identity
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
