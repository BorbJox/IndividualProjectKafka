﻿using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProducerConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProducerConsoleApp.Services
{
    internal class KafkaProducer
    {
        private ILogger _logger;

        public KafkaProducer(ILogger logger)
        {
            _logger = logger;
        }

        public void StartProducing()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            List<int> games = new List<int>()
            {
                100,
                101,
                102,
                103,
                104,
                105,
                106,
                107,
                108,
                109,
                110
            };

            BetGenerator generator = new BetGenerator(0, 100000, 110000, 5000, 1500.0, games);

            using (var producer = new ProducerBuilder<string, string>(configuration.GetSection("KafkaSettings").AsEnumerable(true)).Build())
            {
                var numProduced = 0;
                Random rnd = new Random();

                _logger.LogInformation("Generating bets... Press Ctrl+C to stop.");
                try
                {
                    while (true)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            Bet bet = generator.GenerateBet();
                            string jsonString = JsonSerializer.Serialize<Bet>(bet);

                            producer.Produce("placed_bets", new Message<string, string> { Key = bet.Id.ToString(), Value = jsonString },
                                (deliveryReport) =>
                                {
                                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                                    {
                                        _logger.LogError($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                    }
                                    else
                                    {
                                        _logger.LogDebug($"Produced event to topic placed_bets: key = {bet.Id,-10} value = {jsonString}");
                                        numProduced += 1;
                                    }
                                });

                            Task.Run(() => CompleteBetAsync(generator, bet, rnd, producer));
                            //_ = CompleteBetAsync(generator, bet, rnd, producer);
                        }
                        Thread.Sleep(1000);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl+C was pressed
                }
                finally
                {
                    producer.Flush(TimeSpan.FromSeconds(10));
                    _logger.LogInformation($"{numProduced} messages were produced to topics");
                }
            }
        }

        static async Task CompleteBetAsync(BetGenerator generator, Bet bet, Random random, IProducer<string, string> producer)
        {
            try
            {
                await Task.Delay(random.Next(500));
                Bet completedBet = generator.CompleteBet(bet);
                string jsonString = JsonSerializer.Serialize<Bet>(completedBet);

                producer.Produce("settled_bets", new Message<string, string> { Key = bet.Id.ToString(), Value = jsonString },
                                (deliveryReport) =>
                                {
                                    if (deliveryReport.Error.Code != ErrorCode.NoError)
                                    {
                                        //For async functions we can create a static class which can be assigned a logger (Like Serilog's Log class)
                                        Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                    }
                                    else
                                    {
                                        //Console.WriteLine($"Completed bet: key = {completedBet.Id,-10} amount = {completedBet.WinAmount}");
                                    }
                                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to settle Bet {bet.Id}: {e}");
            }
        }
    }
}