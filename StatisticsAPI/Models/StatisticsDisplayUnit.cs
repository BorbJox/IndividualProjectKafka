namespace StatisticsAPI.Models
{
    public class StatisticsDisplayUnit
    {
        public int GameId { get; set; }
        public int BetCount { get; set; }
        public int StakeSum { get; set; }
        public int WinSum { get; set; }
        public int BiggestWin { get; set; }
    }
}