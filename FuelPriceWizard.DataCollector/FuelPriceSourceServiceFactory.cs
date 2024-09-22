using FuelPriceWizard.BusinessLogic.Modules.Exceptions;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace FuelPriceWizard.DataCollector
{
    public sealed class FuelPriceSourceServiceFactory
    {
        public static List<IFuelPriceSourceFacade> GetFuelPriceSourceServices(IConfiguration config)
        {
            var result = new List<IFuelPriceSourceFacade>();

            var configSections = config.GetSection("ImplementationAssemblies")
                .Get<List<AssemblyDefinition>>() ?? [];

            using var logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();

            foreach (var configSection in configSections)
            {
                var assemblyName = configSection.Type;
                var assemblyFilePath = configSection.FilePath;

                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(assemblyFilePath))
                {
                    throw new FuelPriceWizardLogicException($"Assembly for FuelPriceSourceService implementation of Type \"{assemblyName}\"was not defined in configuration.");
                }

                try
                {

                    var assembly = Assembly.LoadFrom(assemblyFilePath);
                    var type = assembly.GetType(assemblyName);

                    if (type is null)
                    {
                        logger.Error("Something went wrong while parsing assembly type {0}.", assemblyName);
                        continue;
                    }

                    var instanceType = typeof(FuelPriceSourceFacade<>).MakeGenericType(type);

                    result.Add((IFuelPriceSourceFacade) (Activator.CreateInstance(instanceType, config) ?? throw new FuelPriceWizardLogicException("Failed to create instance of FuelPriceSourceFacade.")));
                } catch(Exception ex)
                {
                    logger.Error("Something went wrong while loading the assembly ({0}). {1} {2} {3}", assemblyFilePath, ex.Message, ex.StackTrace, ex.InnerException);
                }
            }

            return result;
        }
    }
}
