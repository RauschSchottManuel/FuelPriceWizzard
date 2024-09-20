using FuelPriceWizard.BusinessLogic.Modules.Exceptions;
using FuelPriceWizard.DataCollector.ConfigDefinitions;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FuelPriceWizard.DataCollector
{
    public sealed class FuelPriceSourceServiceFactory
    {
        public static List<IFuelPriceSourceFacade> GetFuelPriceSourceServices(IConfiguration config)
        {
            var result = new List<IFuelPriceSourceFacade>();

            var configSections = config.GetSection("ImplementationAssemblies")
                .Get<List<AssemblyDefinition>>();

            foreach (var configSection in configSections)
            {
                var assemblyName = configSection.Type;
                var assemblyFilePath = configSection.FilePath;

                if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(assemblyFilePath))
                {
                    throw new FuelPriceWizardLogicException($"Assembly for FuelPriceSourceService implementation of Type \"{assemblyName}\"was not defined in configuration.");
                }

                var assembly = Assembly.LoadFrom(assemblyFilePath);
                var type = assembly.GetType(assemblyName);

                var instanceType = typeof(FuelPriceSourceFacade<>).MakeGenericType(type);

                result.Add((IFuelPriceSourceFacade)(Activator.CreateInstance(instanceType, config) ?? throw new FuelPriceWizardLogicException("Failed to create instance of FuelPriceSourceFacade.")));
            }

            return result;
        }
    }
}
