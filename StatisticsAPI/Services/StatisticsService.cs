using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public interface IStatisticsService
    {
        IEnumerable<StatisticsDisplayUnit> GetStatisticsForTimePeriod(long from, long to, int? gameId = null);
    }

    public class StatisticsService : IStatisticsService
    {
        readonly IStatisticsUnitRepository _repo;

        public StatisticsService(IStatisticsUnitRepository unitRepository)
        {
            _repo = unitRepository;

        }
        public IEnumerable<StatisticsDisplayUnit> GetStatisticsForTimePeriod(long from, long to, int? gameId = null)
        {
            IEnumerable<StatisticsUnit> units = _repo.GetStatisticsUnitsAtTime(from, to, gameId).Result;

            return units.GroupBy(s => s.GameId).Select(s => new StatisticsDisplayUnit
            {
                GameId = s.Key,
                BetCount = s.Sum(g => g.BetCount),
                StakeSum = s.Sum(g => g.StakeSum),
                WinSum = s.Sum(g => g.WinSum),
                BiggestWin = s.Max(g => g.BiggestWin)
            });
        }
    }
}
