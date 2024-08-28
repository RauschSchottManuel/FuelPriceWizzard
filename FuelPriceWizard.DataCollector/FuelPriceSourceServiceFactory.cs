using FuelPriceWizard.BusinessLogic;
using FuelPriceWizard.BusinessLogic.Modules.Exceptions;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FuelPriceWizard.DataCollector
{
    public sealed class FuelPriceSourceServiceFactory
    {
        public static IFuelPriceSourceFacade GetFuelPriceSourceService(IConfiguration config)
        {
            var configSection = config.GetSection("ImplementationAssembly");
            var assemblyName = configSection?.GetValue<string>("Type");
            var assemblyFilePath = configSection?.GetValue<string>("FilePath");

            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(assemblyFilePath))
            {
                throw new FuelPriceWizardLogicException("Assembly for FuelPriceSourceService implementation was not defined in configuration.");
            }

            var assembly = Assembly.LoadFrom(assemblyFilePath);
            var type = assembly.GetType(assemblyName);

            var instanceType = typeof(FuelPriceSourceFacade<>).MakeGenericType(type);

            return (IFuelPriceSourceFacade)(Activator.CreateInstance(instanceType, config) 
                ?? throw new FuelPriceWizardLogicException("Failed to create instance of FuelPriceSourceFacade."));
        }
    }
}
