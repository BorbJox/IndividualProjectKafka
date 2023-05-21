using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StatisticsAPI.Data;
using StatisticsAPI.Models;
using StatisticsAPI.Services;

namespace StatisticsAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class StatisticsController : ControllerBase
    {
        readonly ILogger<StatisticsController> _logger;
        readonly IStatisticsService _service;

        public StatisticsController(ILogger<StatisticsController> logger, IStatisticsService statisticsService)
        {
            _logger = logger;
            _service = statisticsService;
        }

        [HttpGet]
        public IEnumerable<StatisticsDisplayUnit> Get(int from = 0, int to = int.MaxValue, int? gameId = null)
        {
            return _service.GetStatisticsForTimePeriod(from, to, gameId);
        }   

        //[HttpGet]
        //public IEnumerable<MaxWin> BiggestWins(int from = 0, int to = int.MaxValue, int? limit = null)
        //{
        //    var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

        //    var wins = units.GroupBy(s => s.GameId)
        //        .Select(s => new MaxWin
        //        {
        //            GameId = s.Key,
        //            BiggestWin = s.Max(g => g.BiggestWin)
        //        })
        //        .ToList()
        //        .OrderByDescending(s => s.BiggestWin);

        //    if (limit != null)
        //    {
        //        return wins.Skip(0).Take((int)limit);
        //    }

        //    return wins;
        //}

        //[HttpGet]
        //public IEnumerable<BetCount> MostBets(int from = 0, int to = int.MaxValue, int? limit = null)
        //{
        //    var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

        //    var betCount = units.GroupBy(s => s.GameId)
        //        .Select(s => new BetCount
        //        {
        //            GameId = s.Key,
        //            TotalBets = s.Sum(g => g.BetCount),
        //        })
        //        .ToList()
        //        .OrderByDescending(s => s.TotalBets);

        //    if (limit != null)
        //    {
        //        return betCount.Skip(0).Take((int)limit);
        //    }

        //    return betCount;
        //}

        //[HttpGet]
        //public IEnumerable<WinSum> TotalWin(int from = 0, int to = int.MaxValue, int? limit = null)
        //{
        //    var units = _context.StatisticsUnits.Where(s => s.TimePeriod >= from && s.TimePeriod <= to);

        //    var winSum = units.GroupBy(s => s.GameId)
        //        .Select(s => new WinSum
        //        {
        //            GameId = s.Key,
        //            Total = s.Sum(g => g.WinSum),
        //        })
        //        .ToList()
        //        .OrderByDescending(s => s.Total);

        //    if (limit != null)
        //    {
        //        return winSum.Skip(0).Take((int)limit);
        //    }

        //    return winSum;
        //}
    }
}