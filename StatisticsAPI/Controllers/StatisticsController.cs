using Microsoft.AspNetCore.Mvc;
using StatisticsAPI.Data;
using StatisticsAPI.Models;

namespace StatisticsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        readonly ILogger<StatisticsController> _logger;
        readonly StatisticsUnitContext _context;

        public StatisticsController(ILogger<StatisticsController> logger, StatisticsUnitContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet(Name = "GetStatistics")]
        public IQueryable Get(int from = 0, int to = int.MaxValue, int? gameId = null)
        {
            var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

            if (gameId != null)
            {
                units = units.Where(s => s.GameId == gameId);
            }

            return units.GroupBy(s => s.GameId).Select(s => new
            {
                GameId = s.Key,
                SumBets = s.Sum(g => g.BetCount),
                MaxWin = s.Max(g => g.BiggestWin),
                SumWins = s.Sum(g => g.WinSum),
                StatisticsUnitCount = s.Count(),
            });
        }   

        [HttpGet("/biggestWins")]
        public IEnumerable<MaxWin> BiggestWins(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

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

        [HttpGet("/mostBets")]
        public IEnumerable<BetCount> MostBets(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

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

        [HttpGet("/totalWin")]
        public IEnumerable<WinSum> TotalWin(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

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