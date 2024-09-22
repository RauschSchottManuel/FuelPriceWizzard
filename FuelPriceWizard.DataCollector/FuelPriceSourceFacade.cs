using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FuelPriceWizard.DataCollector
{
    public class FuelPriceSourceFacade<T> : IFuelPriceSourceFacade where T : class, IFuelPriceSourceService
    {
        private readonly IFuelPriceSourceService service;

        public FuelPriceSourceFacade(IConfiguration config)
        {
            var assemblySections = config.GetSection("ImplementationAssemblies")
                .Get<List<AssemblyDefinition>>() ?? [];

            var assemblySection = assemblySections.FirstOrDefault(s => s.Type == typeof(T).FullName);

            if(assemblySection is null)
            {
                throw new NullReferenceException($"No assembly entry for {typeof(T).FullName} was found!");
            }

            var assemblyName = assemblySection.Type;

            var assemblyConfig = new ConfigurationBuilder()
                .AddConfiguration(config)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{assemblyName.Split('.')[^1]}.json", optional: true, reloadOnChange: true)
                .Build();

            var logger = new LoggerConfiguration().ReadFrom.Configuration(assemblyConfig)
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(assemblyConfig)
                .AddLogging(builder => builder.AddSerilog(logger: logger, dispose: true))
                .AddFuelPriceWizardDataAccess(assemblyConfig.GetConnectionString("Default")!)
                .AddHttpClient()
                .AddScoped<IFuelPriceSourceService, T>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            service = serviceProvider.GetService<IFuelPriceSourceService>()!;
        }

        public object FetchPricesByLocation(int lat, int lon)
        {
            return service.FetchPricesByLocation(lat, lon);
        }

        public object FetchPricesByLocationAndFuelType(int lat, int lon, FuelType fuelType, int maxReturnResults = 1)
        {
            return service.FetchPricesByLocationAndFuelType(lat, lon, fuelType, maxReturnResults);
        }

        public IConfigurationSection GetFetchSettingsSection()
        {
            return service.GetFetchSettingsSection();
        }
    }
}
