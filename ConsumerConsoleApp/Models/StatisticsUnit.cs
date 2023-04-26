using Microsoft.EntityFrameworkCore;

namespace ProducerConsoleApp.Models
{
    [PrimaryKey(nameof(TimePeriod), nameof(GameId))]
    internal class StatisticsUnit
    {
        public long TimePeriod { get; set; }
        public int GameId { get; set; }
        public int BetCount { get; set; }
        public int StakeSum { get; set; }
        public int WinSum { get; set; }
        public int BiggestWin { get; set; }
    }
}
