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

        [HttpGet]
        public IEnumerable<MaxWin> GetBiggestWins(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            return _service.GetBiggestWins(from, to, limit);
        }

        [HttpGet]
        public IEnumerable<BetCount> MostBets(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            return _service.GetMostBets(from, to, limit);
        }

        [HttpGet]
        public IEnumerable<WinSum> TotalWin(int from = 0, int to = int.MaxValue, int? limit = null)
        {
            return _service.GetTotalWin(from, to, limit);
        }
    }
}