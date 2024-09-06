using FuelPriceWizard.DataAccess.Entities.Mapping;
using FuelPriceWizard.DataAccess.Implementation;
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

            services.AddAutoMapper(
                typeof(AddressMappingProfile),
                typeof(CurrencyMappingProfile),
                typeof(FuelTypeMappingProfile),
                typeof(GasStationMappingProfile),
                typeof(OpeningHoursMappingProfile),
                typeof(PriceReadingMappingProfile)
            );

            //services.AddScoped<IPriceRepository, PriceRepository>();

            return services;
        }
    }
}
