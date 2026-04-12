using FuelPriceWizard.DataAccess.Entities.Mapping;
using FuelPriceWizard.DataAccess.Implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FuelPriceWizard.DataAccess
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection AddFuelPriceWizardDataAccess(this IServiceCollection services, string connectionString, bool isDevelopment = false)
        {
            services.AddDbContext<FuelPriceWizardDbContext>(o =>
            {
                o.UseSqlServer(connectionString);
                if (isDevelopment)
                {
                    o.EnableDetailedErrors();
                    o.EnableSensitiveDataLogging();
                }
            });

            services.AddAutoMapper(
                typeof(AddressMappingProfile),
                typeof(CurrencyMappingProfile),
                typeof(FuelTypeMappingProfile),
                typeof(GasStationMappingProfile),
                typeof(OpeningHoursMappingProfile),
                typeof(PriceReadingMappingProfile)
            );

            services.AddScoped<IPriceRepository, PriceRepository>();
            services.AddScoped<IFuelTypeRepository, FuelTypeRepository>();
            services.AddScoped<ICurrencyRepository, CurrencyRepository>();
            services.AddScoped<IGasStationRepository, GasStationRepository>();

            return services;
        }
    }
}
