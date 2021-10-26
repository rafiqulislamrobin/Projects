using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataImporter.Info;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataImporter.Info.Context;
using Microsoft.EntityFrameworkCore;
using DataImporter.Common;

namespace DataImporter.ImportWorker
{
    public class Program
    {



        private static string _connectionString;
        private static string _migrationAssemblyName;
        private static IConfiguration _configuration;

        public static ILifetimeScope AutofacContainer { get; set; }
        public static void Main(string[] args)
        {
            _configuration = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", false)
           .AddEnvironmentVariables()
           .Build();



            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(_configuration)
                .CreateLogger();
            try
            {
                Log.Information("Application Starting up");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .UseSerilog()
                .ConfigureContainer<ContainerBuilder>(builder =>
                {

                    builder.RegisterModule(new DataImporterModule
                        (_connectionString, _migrationAssemblyName));
                    builder.RegisterModule(new WorkerModule
                        (_connectionString, _migrationAssemblyName,_configuration ));
                    builder.RegisterModule(new CommonModule());

                })
                .ConfigureServices((hostContext, services) =>
                {
                    _connectionString = hostContext.Configuration["ConnectionStrings:DefaultConnection"];

                    _migrationAssemblyName = typeof(Worker).Assembly.FullName;

                    services.AddHostedService<Worker>();

                    services.AddDbContext<DataImporterDbContext>(options =>
                       options.UseSqlServer(_connectionString,
                         b => b.MigrationsAssembly(_migrationAssemblyName)));

                    //services.AddTransient<IStockService, StockService>();
                    //services.AddTransient<StockModel>();

                });




    }
}
