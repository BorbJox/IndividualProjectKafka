using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProducerConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerConsoleApp.Services
{
    internal class StatisticsRepository
    {
        private readonly DbContextOptions<StatisticsUnitContext> _options;

        public StatisticsRepository(string connectionString)
        {
            _options = new DbContextOptionsBuilder<StatisticsUnitContext>()
            .UseSqlServer(connectionString)
            .Options;
        }

        public StatisticsUnit? GetStatisticsUnit(long timestamp, int gameId)
        {
            return new StatisticsUnitContext(_options).StatisticsUnits.Find(timestamp, gameId);
        }

        public void WriteStatisticsUnit(StatisticsUnit unit)
        {
            var unitContext = new StatisticsUnitContext(_options);
            var foundUnit = GetStatisticsUnit(unit.TimePeriod, unit.GameId);
            if (foundUnit != null)
            {
                foundUnit.WinSum += unit.WinSum;
                foundUnit.StakeSum += unit.StakeSum;
                foundUnit.BetCount += unit.BetCount;
                if (unit.BiggestWin > foundUnit.BiggestWin)
                {
                    foundUnit.BiggestWin = unit.BiggestWin;
                }

                unitContext.Update(foundUnit);
            } else
            {
                unitContext.Add(unit);
            }

            unitContext.SaveChanges();
        }
    }
}
