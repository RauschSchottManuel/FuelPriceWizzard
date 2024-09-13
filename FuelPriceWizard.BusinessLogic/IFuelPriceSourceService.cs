using FuelPriceWizard.BusinessLogic.Modules.Enums;

namespace FuelPriceWizard.BusinessLogic
{
    public interface IFuelPriceSourceService
    {
        object FetchPricesByLocation(int lat, int lon);
        object FetchPricesByLocationAndFuelType(int lat, int lon, FuelType fuelType, int maxReturnResults = 1);

        //TODO: define interface for fetching a data source for fuel prices
    }
}
