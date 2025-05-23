using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext : DbContext
{
    public DbSet<UserData> Data { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=UtilityBillDatabase.db");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}