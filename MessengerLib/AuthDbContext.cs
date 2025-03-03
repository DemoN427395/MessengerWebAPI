using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MessengerLib.Models;

namespace MessengerLib.Data;

// DbContext for Authentication and Token information
public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<TokenInfo> TokenInfos { get; set; }

    // Configure the models for AuthDbContext
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("user");

        // Configure ApplicationUser entity
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(u => u.Name).HasMaxLength(100);
        });

        // Configure TokenInfo entity
        modelBuilder.Entity<TokenInfo>()
            .ToTable("TokenInfos");
    }
}