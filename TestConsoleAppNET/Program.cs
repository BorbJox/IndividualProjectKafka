using Microsoft.Extensions.Configuration;
using Confluent.Kafka;
using ProducerConsoleApp.Services;
using ProducerConsoleApp.Models;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ProducerConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("ProducerConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();
            logger.LogInformation("Staring The Program!");

            var producer = new KafkaProducer(logger);
            producer.StartProducing();

        }
    }
}