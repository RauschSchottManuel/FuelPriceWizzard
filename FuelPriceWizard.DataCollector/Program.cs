using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataAccess.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FuelPriceWizard.DataCollector
{
    internal class Program
    {
        protected Program() { }

        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((ctx, services) =>
                {
                    var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");

                    services.RegisterServiceCollections(configPath);

                }).Build();

            var orchestrator = host.Services.GetRequiredService<IDataCollectorOrchestrator>();

            orchestrator.CreateTasks();

            orchestrator.StartTasks();

            await host.RunAsync();
        }
    }

    public static class HostRegistrationHelper
    {
        public static IServiceCollection RegisterServiceCollections(this IServiceCollection services, string? configPath)
        {
            var appsettingsPath = configPath is not null ? $"{configPath}/appsettings.json" : "appsettings.json";

            var isBaseConfigPathNull = configPath is not null;

            var configPathEntry = new Dictionary<string, string?>
            {
                { "UseExternalBaseConfigPath", isBaseConfigPathNull.ToString() },
                { "ExternalBaseConfigPath", $"{configPath}" }
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(appsettingsPath, reloadOnChange: true, optional: false)
                .AddInMemoryCollection(configPathEntry)
               .Build();

            using var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            services.AddSingleton<IConfiguration>(config)
                .AddLogging(builder => builder.AddSerilog(logger: logger, dispose: true))
                .AddFuelPriceWizardDataAccess(config.GetConnectionString(ConnectionStringConstants.FUEL_PRICE_WIZARD)!)
                .AddScoped<IDataCollectorOrchestrator, DataCollectorOrchestrator>();

            return services;
        }
    }

}
