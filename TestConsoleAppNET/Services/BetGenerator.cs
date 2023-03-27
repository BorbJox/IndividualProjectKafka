using ProducerConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsoleApp.Services
{
    internal class BetGenerator
    {
        private readonly int _userIdsFrom;
        private readonly int _userIdsTo;
        private readonly int _maxBetAmount;
        private readonly double _maxWinMultiplier;
        private readonly List<int> _gameIds;
        private int _lastId;
        private Random _random;

        public BetGenerator(int lastId, int userIdsFrom, int userIdsTo, int maxBetAmount, double maxWinMultiplier, List<int> gameIds) 
        {
            _lastId = lastId;
            _userIdsFrom = userIdsFrom;
            _userIdsTo = userIdsTo;
            _maxBetAmount = maxBetAmount;
            _maxWinMultiplier = maxWinMultiplier;
            _gameIds = gameIds;
            _random = new Random();
        }

        public Bet GenerateBet()
        {
            int index = _random.Next(_gameIds.Count);
            int gameId = _gameIds[index];

            Bet newBet = new Bet
            {
                Id = ++_lastId,
                GameId = gameId,
                UserId = _random.Next(_userIdsFrom, _userIdsTo),
                StakeAmount = _random.Next(1, _maxBetAmount),
                WinAmount = null, //Bet is still not settled when creating
                Created = DateTime.UtcNow
            };

            return newBet;
        }

        public Bet CompleteBet(Bet bet)
        {
            bool won = _random.NextDouble() > 0.5;
            int winAmount = 0;
            if (won)
            {
                //Lower the odds of getting a high win with Math.Pow
                winAmount = Convert.ToInt32(Math.Pow(_random.NextDouble(), 5.0) * _maxWinMultiplier * bet.StakeAmount);
            }

            bet.WinAmount = winAmount;
            return bet;
        }
    }
}
