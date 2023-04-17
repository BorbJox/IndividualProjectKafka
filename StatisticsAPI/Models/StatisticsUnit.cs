namespace StatisticsAPI.Models
{
    public class StatisticsUnit
    {
        public long TimePeriod { get; set; }
        public int GameId { get; set; }
        public int BetCount { get; set; }
        public int StakeSum { get; set; }
        public int WinSum { get; set; }
        public int BiggestWin { get; set; }
    }
}
