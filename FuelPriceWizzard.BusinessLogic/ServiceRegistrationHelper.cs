using FuelPriceWizard.BusinessLogic.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace FuelPriceWizard.BusinessLogic
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection AddFuelPriceWizardBusinessLogic(this IServiceCollection services)
        {
            services.AddScoped<IFuelPriceWizardService, FuelPriceWizardService>();

            return services;
        }
    }
}
