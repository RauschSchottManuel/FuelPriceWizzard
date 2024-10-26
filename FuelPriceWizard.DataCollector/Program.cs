using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        protected Program() { }

        static void Main(string[] args)
        {
            var serviceProvider = CreateServiceCollection().BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
            var orchestrator = serviceProvider.GetRequiredService<IDataCollectorOrchestrator>();

            logger.LogInformation("DataCollector started!");

            orchestrator.CreateTasks();

            orchestrator.StartTasks();

            Console.ReadKey();
        }

        static IServiceCollection CreateServiceCollection()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
               .Build();

            using var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            return new ServiceCollection()
                .AddSingleton<IConfiguration>(config)
                .AddLogging(builder => builder.AddSerilog(logger: logger, dispose: true))
                .AddFuelPriceWizardDataAccess(config.GetConnectionString(ConnectionStringConstants.FUEL_PRICE_WIZARD)!)
                .AddScoped<IDataCollectorOrchestrator, DataCollectorOrchestrator>();
        }
    }

}
