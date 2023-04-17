using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using StatisticsAPI.Data;
using StatisticsAPI.Models;

namespace StatisticsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) //To hide ASP.NET request logging (Will override with Serilog in line 38)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("type", "statistics-api")
                .WriteTo.Debug(new CompactJsonFormatter())
                .CreateLogger();

            try
            {
                Log.Information("Starting the ASP.NET app!");

                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Host.UseSerilog(); //Will override all Logging

                //This is how normally Microsoft would want you to use its ASP.NET Logging:
                
                //builder.Logging.AddConsole(); //But this specific line would be pointless, because
                /* WebApplication.CreateBuilder adds the following logging providers by default:
                    Console
                    Debug
                    EventSource
                    EventLog: Windows only
                */
                //builder.Logging.AddSerilog(); //Would just add Serilog in addition to other logging providers. 

                builder.Services.AddControllers(config =>
                {
                    //config.Filters.Add(new TestGlobalActionFilter());
                });
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Services.AddDbContext<StatisticsUnitContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL")));
                builder.Services.AddDatabaseDeveloperPageExceptionFilter();

                var app = builder.Build();

                //Overrides default ASP.NET request logging.
                app.UseSerilogRequestLogging();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Something bad happned ._.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}