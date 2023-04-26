namespace ProducerConsoleApp.Models
{
    internal class Bet
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
        public int StakeAmount { get; set; }
        public int? WinAmount { get; set; }
        public DateTime Created { get; set; }
    }

}
