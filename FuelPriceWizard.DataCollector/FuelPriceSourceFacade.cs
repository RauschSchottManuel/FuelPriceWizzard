﻿using FuelPriceWizard.BusinessLogic;
using Enums = FuelPriceWizard.BusinessLogic.Modules.Enums;
using FuelPriceWizard.DataAccess;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using FuelPriceWizard.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using FuelPriceWizard.BusinessLogic.Modules.Exceptions;
using FuelPriceWizard.DataAccess.Constants;

namespace FuelPriceWizard.DataCollector
{
    public class FuelPriceSourceFacade<T> : IFuelPriceSourceFacade where T : class, IFuelPriceSourceService
    {
        private readonly IFuelPriceSourceService service;

        public FuelPriceSourceFacade(IConfiguration config)
        {
            var baseConfigPath = config.GetValue<bool>("UseExternalBaseConfigPath") ? $"{config.GetValue<string?>("ExternalBaseConfigPath")}/" : null;

            var assemblySections = config.GetSection("ImplementationAssemblies")
                .Get<List<AssemblyDefinition>>() ?? [];

            var assemblySection = assemblySections.Find(s => s.Type == typeof(T).FullName)
                ?? throw new FuelPriceWizardLogicException($"No assembly entry for {typeof(T).FullName} was found!");

            var assemblyName = assemblySection.Type;

            var assemblyConfig = new ConfigurationBuilder()
                .AddConfiguration(config)
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"{baseConfigPath ?? string.Empty}appsettings.{assemblyName.Split('.')[^1]}.json", optional: true, reloadOnChange: true)
                .Build();

            var logger = new LoggerConfiguration().ReadFrom.Configuration(assemblyConfig)
                .CreateLogger();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(assemblyConfig)
                .AddLogging(builder => builder.AddSerilog(logger: logger, dispose: true))
                .AddFuelPriceWizardDataAccess(assemblyConfig.GetConnectionString(ConnectionStringConstants.FUEL_PRICE_WIZARD)!)
                .AddHttpClient()
                .AddScoped<IFuelPriceSourceService, T>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
            service = serviceProvider.GetService<IFuelPriceSourceService>()!;
        }

        public async Task Setup() => await service.Setup();

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true)
        {
            return await service.FetchPricesByLocationAsync(lat, lon, includeClosed);
        }

        public async Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, Enums.FuelType fuelType, bool includeClosed = true)
        {
            return await service.FetchPricesByLocationAndFuelTypeAsync(lat, lon, fuelType, includeClosed);
        }

        public IConfigurationSection GetFetchSettingsSection()
        {
            return service.GetFetchSettingsSection();
        }
    }
}
