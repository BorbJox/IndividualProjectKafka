using Microsoft.EntityFrameworkCore;
using StatisticsAPI.Models;
using StatisticsAPI.Repositories;

namespace StatisticsAPI.Services
{
    public interface IStatisticsService
    {
        IEnumerable<StatisticsDisplayUnit> GetStatisticsForTimePeriod(long from, long to, int? gameId = null);
        IEnumerable<MaxWin> GetBiggestWins(int from = 0, int to = int.MaxValue, int? limit = null);
        IEnumerable<BetCount> GetMostBets(int from = 0, int to = int.MaxValue, int? limit = null);
        IEnumerable<WinSum> GetTotalWin(int from = 0, int to = int.MaxValue, int? limit = null);
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

        public IEnumerable<MaxWin> GetBiggestWins(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _repo.GetStatisticsUnitsAtTime(from, to).Result;

            var wins = units.GroupBy(s => s.GameId)
                .Select(s => new MaxWin
                {
                    GameId = s.Key,
                    BiggestWin = s.Max(g => g.BiggestWin)
                })
                .ToList()
                .OrderByDescending(s => s.BiggestWin);

            if (limit != null)
            {
                return wins.Skip(0).Take((int)limit);
            }

            return wins;
        }

        public IEnumerable<BetCount> GetMostBets(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _repo.GetStatisticsUnitsAtTime(from, to).Result;

            var betCount = units.GroupBy(s => s.GameId)
                .Select(s => new BetCount
                {
                    GameId = s.Key,
                    TotalBets = s.Sum(g => g.BetCount),
                })
                .ToList()
                .OrderByDescending(s => s.TotalBets);

            if (limit != null)
            {
                return betCount.Skip(0).Take((int)limit);
            }

            return betCount;
        }

        public IEnumerable<WinSum> GetTotalWin(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _repo.GetStatisticsUnitsAtTime(from, to).Result;

            var winSum = units.GroupBy(s => s.GameId)
                .Select(s => new WinSum
                {
                    GameId = s.Key,
                    Total = s.Sum(g => g.WinSum),
                })
                .ToList()
                .OrderByDescending(s => s.Total);

            if (limit != null)
            {
                return winSum.Skip(0).Take((int)limit);
            }

            return winSum;
        }
    }
}
