using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ProducerConsoleApp.Models;
using Sentry;
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
            //Things to use...
            //TODO: User secrets
            //TODO: Action Filters for API
            //TODO: Validation using attributes for API
            //TODO: Benchmark how long it takes to consume https://github.com/dotnet/BenchmarkDotNet
            //TODO: Simplify Program.cs

            //SqlConnection con = new SqlConnection(configuration.GetConnectionString("MSSQL"));

            //SqlCommand cmd = new SqlCommand("SELECT * FROM [Events]", con);
            //cmd.CommandType = CommandType.Text;
            //con.Open();
            //SqlDataReader rdr = cmd.ExecuteReader();

            //while (rdr.Read())
            //{
            //    Console.WriteLine(rdr.GetString("Team1"));
            //}

            /*
             * TODO: Pick smallest time resolution (1 second? minute?)
             * TODO: Maybe get the 10th biggest win for the top 10? What about time resolutions? 
             * Might need to do it "top 10 this second". Or hour. 
             * But practically it'll be top 10 today.
             * 
             * Gather per game:
             *     - Bet Count
             *     - Stake Sum
             *     - Biggest 10 wins
             *     - Win Sum
             *     
             *     Consider: queue management
             *     
             * DB Structure:
             *    timestamp
             *    bet count
             *    stake sum
             *    win sum
             * 
             */
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
            /*
             * generate/update StatisticsUnit
             * when found a bet for a new second, with a delay add the dictionary at the new timestamp to a task to be written
             * Async write/update to DB what's at the determined timestamp
             * 
             * So:
             * 1. Find a bet for new timestamp
             * 2. Create a task that will trigger after a delay
             * 3. It will look at workingData at the given timestamp
             * 4. It will take it to memory and delete from workingData (use locks, or ConcurrentDictionary)
             * 5. It will write things to Database on another thread
             * 
             */

            using (var consumer = new ConsumerBuilder<string, string>(kafkaConfig).Build())
            {
                repository.GetStatisticsUnit(123, 100);
                consumer.Subscribe(topics);
                try
                {
                    Console.WriteLine("Starting to listen...");
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

                        Console.WriteLine($"Consumed event {cr.Topic} with key {cr.Message.Key,-10} and value {cr.Message.Value}");
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
            //TODO: Put it back if failed to write to DB (rewrite as service)

            Console.Out.WriteLine($"Writing Statistics at timestamp {timestamp} now.");
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
