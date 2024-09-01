using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FuelPriceWizard.DataAccess
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection AddFuelPriceWizardDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FuelPriceWizardDbContext>(o =>
            {
                o.UseSqlServer(connectionString);
                o.EnableDetailedErrors();
            });
            return services;
        }
    }
}
