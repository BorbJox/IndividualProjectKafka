using Microsoft.EntityFrameworkCore;

namespace ProducerConsoleApp.Models
{
    internal class StatisticsUnitContext : DbContext
    {
        public StatisticsUnitContext(DbContextOptions<StatisticsUnitContext> options)
        : base(options)
        {
        }
        public DbSet<StatisticsUnit> StatisticsUnits { get; set; }
    }
}
