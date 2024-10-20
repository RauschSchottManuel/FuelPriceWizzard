using FuelPriceWizard.BusinessLogic.Modules.Exceptions;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace FuelPriceWizard.DataCollector
{
    public sealed class FuelPriceSourceServiceFactory
    {
        private FuelPriceSourceServiceFactory() { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3885:\"Assembly.Load\" should be used", Justification = "<Pending>")]
        public static List<IFuelPriceSourceFacade> GetFuelPriceSourceServices(IConfiguration config, ILogger<FuelPriceSourceServiceFactory> logger)
        {
            var result = new List<IFuelPriceSourceFacade>();

            var configSections = config.GetSection("ImplementationAssemblies")
                .Get<List<AssemblyDefinition>>() ?? [];

            logger.LogInformation("Loading collector instances: {Instances}", configSections.Select(s => s.Type));

            foreach (var configSection in configSections)
            {
                if (!configSection.Enabled)
                {
                    logger.LogWarning("Collector {Instance} is disabled.", configSection.Type);
                    continue;
                }

                if (string.IsNullOrEmpty(configSection.Type) || string.IsNullOrEmpty(configSection.FilePath))
                {
                    throw new FuelPriceWizardLogicException(
                        $"Assembly for FuelPriceSourceService implementation of Type \"{configSection.Type}\"was not defined in configuration.");
                }

                try
                {

                    var assembly = Assembly.LoadFrom(configSection.FilePath);
                    var type = assembly.GetType(configSection.Type);

                    if (type is null)
                    {
                        logger.LogError("Something went wrong while parsing assembly type {Type}.", configSection.Type);
                        continue;
                    }

                    var instanceType = typeof(FuelPriceSourceFacade<>).MakeGenericType(type);

                    result.Add((IFuelPriceSourceFacade) (Activator.CreateInstance(instanceType, config)
                        ?? throw new FuelPriceWizardLogicException("Failed to create instance of FuelPriceSourceFacade.")));
                } catch(Exception ex)
                {

                    logger.LogError(ex, "Something went wrong while loading the assembly {FilePath}.",
                        configSection.FilePath);
                }
            }

            logger.LogInformation("Finished loading the following collector instances: {Instances}",
                configSections.Where(s => s.Enabled).Select(s => s.Type));

            return result;
        }
    }
}
