using ConsumerConsoleApp.Services;
using Serilog;

namespace ConsumerConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            Log.Logger = logger;

            var consumer = new KafkaConsumer();
            consumer.StartConsuming();

            Log.CloseAndFlush();
        }
    }
}