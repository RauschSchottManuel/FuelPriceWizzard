using Microsoft.Extensions.DependencyInjection;

namespace FuelPriceWizard.DataAccess
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection AddFuelPriceWizardDataAccess(this IServiceCollection services, string connectionString)
        {
            return services;
        }
    }
}
