using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ProducerConsoleApp.Models;
using Serilog;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Text.Json;

namespace ConsumerConsoleApp.Services
{
    internal class KafkaConsumer
    {
        public void StartConsuming()
        {
            
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("MSSQL") ?? "";
            StatisticsRepository repository = new(connectionString);

            ImmutableArray<string> topics = ImmutableArray.Create("placed_bets", "settled_bets");
            int loopsCount = 0;
            int loopsLimit = 200;

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            var kafkaConfig = configuration.GetSection("KafkaSettings").AsEnumerable(true);

            ConcurrentDictionary<long, Dictionary<int, StatisticsUnit>> workingData = new();

            using (var consumer = new ConsumerBuilder<string, string>(kafkaConfig).Build())
            {
                repository.GetStatisticsUnit(123, 100);
                consumer.Subscribe(topics);
                try
                {
                    Log.Information("Starting to listen...");
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);
                        Bet? betInMessage = JsonSerializer.Deserialize<Bet>(cr.Message.Value);
                        if (betInMessage != null)
                        {
                            long betTimeStamp = new DateTimeOffset(betInMessage.Created).ToUnixTimeSeconds();
                            if (!workingData.TryGetValue(betTimeStamp, out Dictionary<int, StatisticsUnit>? timeDict))
                            {
                                timeDict = new();
                                workingData[betTimeStamp] = timeDict;
                                _ = DelayAction(() => WriteStatisticsToDb(betTimeStamp, ref workingData, repository));
                            }

                            if (workingData[betTimeStamp].TryGetValue(betInMessage.GameId, out StatisticsUnit? unit))
                            {
                                unit.BetCount++;
                                unit.StakeSum += betInMessage.StakeAmount;
                                int win = betInMessage.WinAmount ?? 0;
                                unit.WinSum += win;
                                if (win > unit.BiggestWin)
                                {
                                    unit.BiggestWin = win;
                                }
                            }
                            else
                            {
                                unit = new()
                                {
                                    TimePeriod = betTimeStamp,
                                    GameId = betInMessage.GameId,
                                    BetCount = 1,
                                    StakeSum = betInMessage.StakeAmount,
                                    WinSum = betInMessage.WinAmount ?? 0,
                                    BiggestWin = betInMessage.WinAmount ?? 0
                                };
                                workingData[betTimeStamp].Add(betInMessage.GameId, unit);
                            }
                        }

                        Log.Debug($"Consumed event {cr.Topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
                        if (loopsCount++ > loopsLimit)
                        {
                            Thread.Sleep(1);
                            loopsCount = 0;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
        private static void WriteStatisticsToDb(long timestamp, ref ConcurrentDictionary<long, Dictionary<int, StatisticsUnit>> data, StatisticsRepository repo)
        {
            bool removed = data.TryRemove(timestamp, out var units);

            if (removed && units != null)
            {
                foreach (var statSet in units)
                {
                    repo.WriteStatisticsUnit(statSet.Value);
                }
            }

            Log.Debug($"Writing Statistics at timestamp {timestamp} now.");
        }

        private static async Task DelayAction(Action action)
        {
            await Task.Delay(2000);
            Task task = new(action);
            task.Start();
            await task;
        }
    }
}
