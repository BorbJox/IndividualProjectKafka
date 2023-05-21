using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using StatisticsAPI.Data;
using StatisticsAPI.Repositories;
using StatisticsAPI.Services;

namespace StatisticsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning) //To hide ASP.NET request logging
                .Enrich.FromLogContext()
                .WriteTo.Debug()
                .CreateLogger();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog(); 

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

                builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer();

                builder.Services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminOnly", policy =>
                        policy.RequireClaim("IsAdmin", "1"));
                });

                builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL")));

                builder.Services.AddScoped<IStatisticsUnitRepository, StatisticsUnitRepository>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IStatisticsService, StatisticsService>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IJwtProvider, JwtProvider>();
                builder.Services.AddSingleton<DynamicJwtValidationHandler>();

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

                app.UseAuthentication();
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