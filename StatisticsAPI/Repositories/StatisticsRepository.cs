using Microsoft.EntityFrameworkCore;
using StatisticsAPI.Data;
using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public class StatisticsUnitRepository : IStatisticsUnitRepository
    {
        private readonly AppDbContext appDbContext;

        public StatisticsUnitRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<IEnumerable<StatisticsUnit>> GetStatisticsUnits()
        {
            return await appDbContext.StatisticsUnits.ToListAsync();
        }

        public async Task<IEnumerable<StatisticsUnit>> GetStatisticsUnitsAtTime(long from, long to, int? gameId)
        {
            var qb = appDbContext.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);
            if (gameId != null) {
                qb = qb.Where(s => s.GameId == gameId);
            }
            return await qb.ToListAsync();
        }

        public async Task<StatisticsUnit> GetStatisticsUnit(long timestamp, int gameId)
        {
            return await appDbContext.StatisticsUnits.FirstAsync(s => s.TimePeriod == timestamp && s.GameId == gameId);
        }

        public async Task<StatisticsUnit> AddStatisticsUnit(StatisticsUnit statisticsUnit)
        {
            var result = await appDbContext.StatisticsUnits.AddAsync(statisticsUnit);
            await appDbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<StatisticsUnit?> UpdateStatisticsUnit(StatisticsUnit statisticsUnit)
        {
            StatisticsUnit? result = await appDbContext.StatisticsUnits.FirstOrDefaultAsync(s => s.TimePeriod == statisticsUnit.TimePeriod && s.GameId == statisticsUnit.GameId);

            if (result != null)
            {
                result.TimePeriod = statisticsUnit.TimePeriod;
                result.GameId = statisticsUnit.GameId;
                result.BetCount = statisticsUnit.BetCount;
                result.StakeSum = statisticsUnit.StakeSum;
                result.WinSum = statisticsUnit.WinSum;
                result.BiggestWin = statisticsUnit.BiggestWin;

                await appDbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async void IncrementStatisticsUnit(StatisticsUnit unit)
        {
            var foundUnit = await GetStatisticsUnit(unit.TimePeriod, unit.GameId);
            if (foundUnit != null)
            {
                foundUnit.WinSum += unit.WinSum;
                foundUnit.StakeSum += unit.StakeSum;
                foundUnit.BetCount += unit.BetCount;
                if (unit.BiggestWin > foundUnit.BiggestWin)
                {
                    foundUnit.BiggestWin = unit.BiggestWin;
                }

                await UpdateStatisticsUnit(foundUnit);
            } else
            {
                await AddStatisticsUnit(unit);
            }
        }

    }
}
