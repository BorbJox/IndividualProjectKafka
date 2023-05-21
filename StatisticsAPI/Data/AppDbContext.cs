using Microsoft.EntityFrameworkCore;
using StatisticsAPI.Models;

namespace StatisticsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 
        }

        public DbSet<StatisticsUnit> StatisticsUnits { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<StatisticsUnit>()
                .HasKey(e => new { e.TimePeriod, e.GameId });
        }
    }
}
