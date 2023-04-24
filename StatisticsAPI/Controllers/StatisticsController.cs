using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using StatisticsAPI.Data;
using StatisticsAPI.Models;
using StatisticsAPI.Services;
using System.Linq;

namespace StatisticsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController : ControllerBase
    {
        readonly ILogger<StatisticsController> _logger;
        readonly IDiagnosticContext _diagnosticContext;
        readonly StatisticsUnitContext _context;

        public StatisticsController(ILogger<StatisticsController> logger, IDiagnosticContext diagnosticContext, StatisticsUnitContext context)
        {
            _logger = logger;
            _diagnosticContext = diagnosticContext ?? throw new ArgumentNullException(nameof(diagnosticContext));
            _context = context;
        }

        [HttpGet(Name = "GetStatistics")]
        public IQueryable Get(StatisticsUnitContext context)
        {
            //Diagnostics are used for adding info to the request log entry ("HTTP GET /blabla.html responded 200 in 11.11 ms")
            //Will only appear in JSON formats, not the message string
            _diagnosticContext.Set("Some diagnostic info", new { foo = "bar", interestingNumber = 8 }, true); //true here deconstructs the object (otherwise would call .ToString())
            _diagnosticContext.Set("UserId", 7896);

            using (LogContext.PushProperty("MyDebugObject", new { good = "object" }, true))
            using (LogContext.PushProperty("OneMore", new { another = "thing" }, true))
            {
                _logger.LogInformation("Logging some info now!");
            }
            _logger.LogInformation("I lost my context.. :(");


            var units = _context.StatisticsUnits.Where(s => s.TimePeriod > 1679942618 && s.TimePeriod < 1679942622)
                    .GroupBy(s => s.GameId)
                    .Select(s => new
                    {
                        s.Key,
                        SumBets = s.Sum(g => g.BetCount),
                        MaxWin = s.Max(g => g.BiggestWin),
                        SumWins = s.Sum(g => g.WinSum),
                        count = s.Count(),
                    });

            return units;

            /* TODO: 
             * 1. Query a period of time
             * 2. Sum of bets for each game for that time
             * 3. Max win amount for each game
             * 4. Sum of amount won for a single game for that time
             */

            //return "Hey what's up";
        }
    }
}