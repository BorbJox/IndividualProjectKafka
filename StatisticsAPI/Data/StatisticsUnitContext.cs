using Microsoft.EntityFrameworkCore;
using StatisticsAPI.Models;

namespace StatisticsAPI.Data
{
    public class StatisticsUnitContext : DbContext
    {
        public StatisticsUnitContext(DbContextOptions<StatisticsUnitContext> options)
        : base(options)
        {
        }
        public DbSet<StatisticsUnit> StatisticsUnits { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<StatisticsUnit>()
                .HasKey(e => new { e.TimePeriod, e.GameId });
        }
    }
}
