using ConsumerConsoleApp.Services;
using ProducerConsoleApp.Models;
using System.Collections.Concurrent;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Net.Sockets;
using Sentry;

namespace ConsumerConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //We do need file and logstash logging (or Beats with file logging)
            //.NET doesn't provide a file logger (neither elasticsearch/logstash)
            //We don't want to write our own
            //Thus - we'll likely use a third party library
            //Popular:
            //log4net - is probably oldest? (2001) not much support for structured json logs, docs focused on old tech
            //NLog - fast, can do strucutred, pretty old (2006), but more actively maintained than log4net
            //Serilog - youngest (2013), build for structured logs, more modular than others, no XML configs, I picked this one. Also, bonus team uses it.
            //There's probably more lightweight solutions for simpler logging, but we're not making many "simple" projects.

            //One way to initialize Sentry without Serilog
            //using (SentrySdk.Init(o =>
            //{
            //    // Tells which project in Sentry to send events to:
            //    o.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";
            //    // When configuring for the first time, to see what the SDK is doing:
            //    o.Debug = false;
            //    // Set traces_sample_rate to 1.0 to capture 100% of transactions for performance monitoring.
            //    // We recommend adjusting this value in production.
            //    o.TracesSampleRate = 1.0;
            //}))
            //{
                var logger = new LoggerConfiguration()
                    .MinimumLevel.Debug() //Default is Information. Verbose->Debug->Information->Warning->Error->Fatal (this is for Serilog, non-standard)
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning) //Can raise the log level, never lower.  I don't see much use for console logging. Maybe for real-time displays in office
                    .WriteTo.Async(writeTo => writeTo.File("../logs/consumerAsync.log")) // <<#<<#<<
                    .WriteTo.File(
                        path: "../logs/consumer.log", //Can go up folders
                        rollingInterval: RollingInterval.Hour,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{Properties:j} {Message:lj}{NewLine}{Exception}" //Added Properties
                        )
                    .WriteTo.File(new CompactJsonFormatter(), "../logs/consumerJSON.log")
                    .WriteTo.Http("http://127.0.0.1/", 8192) //8192 byte queue limit (this sink batches multiple log entries into one request)
                    .WriteTo.Udp("127.0.0.1", 31311, AddressFamily.InterNetwork) //For our UDP server
                    .WriteTo.Sentry(o =>
                    {
                        //Will initialize main sentry SDK, an alternative way
                        o.Dsn = "https://examplePublicKey@o0.ingest.sentry.io/0";
                        // Store as breadcrumbs (default is Information)
                        o.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                        // Send logs as event (default is Error)
                        o.MinimumEventLevel = LogEventLevel.Warning;

                        //o.Debug = true;

                        o.MaxBreadcrumbs = 20; //Be wary of sentry maximum payload size
                    })
                    //.Destructure.ByTransforming<Bet>(c => new { c.Id, c.StakeAmount, c.WinAmount, type = "Bet" }) //There's more to this, can use IDestructuringPolicy
                    .CreateLogger();


                //Creating another logger with a context, which will have Properties
                var testLog = logger.ForContext("MyContextName", new Bet() { StakeAmount = 123 }, true); //Probably not going to use Properties anywhere though.
                testLog.Information("Look at the properties");
                testLog.Information("They're still here");


                Log.Logger = logger; //Assigning this logger to the static Log class

                //Can also just send one message with properties from the static Log class
                Log.ForContext<KafkaConsumer>().Information("I have a parameter with my class name!"); //Unnecessary, but can use it to filter log areas

                var consumer = new KafkaConsumer();
                consumer.StartConsuming();

                //Must flush the static class when console app closes. (If using instanced logger objects (e.g. testLog here), destructor will handle them)
                Log.CloseAndFlush();
            //}
        }

        private static void WriteStatisticsToDb(long timestamp, ref ConcurrentDictionary<long, Dictionary<int, StatisticsUnit>> data, StatisticsRepository repo)
        {
            bool removed = data.TryRemove(timestamp, out var units);

            if (removed && units != null)
            {
                foreach(var statSet in units)
                {
                    repo.WriteStatisticsUnit(statSet.Value);
                }
            }
            //TODO: Put it back if failed to write to DB (rewrite as service)

            Console.Out.WriteLine($"Writing Statistics at timestamp {timestamp} now.");
        }

        private static async Task DelayAction(Action action)
        {
            await Console.Out.WriteLineAsync("Will wait 2 secs.");
            await Task.Delay(2000);
            Task task = new(action);
            task.Start();
            await task;
        }
    }
}