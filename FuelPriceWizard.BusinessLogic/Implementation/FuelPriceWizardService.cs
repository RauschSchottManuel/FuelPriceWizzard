using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.BusinessLogic.Implementation
{
    public class FuelPriceWizardService(
        IGasStationRepository gasStationRepository,
        IPriceRepository priceRepository,
        IFuelTypeRepository fuelTypeRepository,
        ICurrencyRepository currencyRepository) : IFuelPriceWizardService
    {
        public Task<IEnumerable<GasStation>> GetAllGasStationsAsync() =>
            gasStationRepository.GetAllAsync();

        public Task<GasStation?> GetGasStationByIdAsync(int id) =>
            gasStationRepository.GetByIdAsync(id);

        public Task<IEnumerable<PriceReading>> GetLatestPricesForStationAsync(int stationId) =>
            priceRepository.GetLatestByStationAsync(stationId);

        public Task<IEnumerable<PriceReading>> GetPriceHistoryAsync(int stationId, int fuelTypeId, DateTime from, DateTime to) =>
            priceRepository.GetHistoryAsync(stationId, fuelTypeId, from, to);

        public Task<IEnumerable<FuelType>> GetAllFuelTypesAsync() =>
            fuelTypeRepository.GetAllAsync();

        public Task<IEnumerable<Currency>> GetAllCurrenciesAsync() =>
            currencyRepository.GetAllAsync();
    }
}
