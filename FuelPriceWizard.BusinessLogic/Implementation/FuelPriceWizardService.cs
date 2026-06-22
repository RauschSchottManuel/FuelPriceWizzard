using FuelPriceWizard.DataAccess;
using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.BusinessLogic.Implementation
{
    public class FuelPriceWizardService(
        IGasStationRepository gasStationRepository,
        IFuelTypeRepository fuelTypeRepository,
        ICurrencyRepository currencyRepository,
        IPriceRepository priceRepository) : IFuelPriceWizardService
    {
        // GasStations
        public Task<(IEnumerable<GasStation> Items, int TotalCount)> GetGasStationsPagedAsync(int page, int pageSize, CancellationToken ct = default)
            => gasStationRepository.GetPagedAsync(page, pageSize, ct);

        public Task<GasStation?> GetGasStationByIdAsync(int id, CancellationToken ct = default)
            => gasStationRepository.GetByIdAsync(id, ct);

        public Task<GasStation> CreateGasStationAsync(GasStation model, CancellationToken ct = default)
            => gasStationRepository.InsertAsync(model, ct);

        public Task<GasStation> UpdateGasStationAsync(int id, GasStation model, CancellationToken ct = default)
            => gasStationRepository.UpdateAsync(id, model, ct);

        public Task<bool> DeleteGasStationAsync(int id, CancellationToken ct = default)
            => gasStationRepository.DeleteByIdAsync(id, ct);

        // FuelTypes
        public Task<(IEnumerable<FuelType> Items, int TotalCount)> GetFuelTypesPagedAsync(int page, int pageSize, CancellationToken ct = default)
            => fuelTypeRepository.GetPagedAsync(page, pageSize, ct);

        public Task<FuelType?> GetFuelTypeByIdAsync(int id, CancellationToken ct = default)
            => fuelTypeRepository.GetByIdAsync(id, ct);

        public Task<FuelType> CreateFuelTypeAsync(FuelType model, CancellationToken ct = default)
            => fuelTypeRepository.InsertAsync(model, ct);

        public Task<FuelType> UpdateFuelTypeAsync(int id, FuelType model, CancellationToken ct = default)
            => fuelTypeRepository.UpdateAsync(id, model, ct);

        public Task<bool> DeleteFuelTypeAsync(int id, CancellationToken ct = default)
            => fuelTypeRepository.DeleteByIdAsync(id, ct);

        // Currencies
        public Task<(IEnumerable<Currency> Items, int TotalCount)> GetCurrenciesPagedAsync(int page, int pageSize, CancellationToken ct = default)
            => currencyRepository.GetPagedAsync(page, pageSize, ct);

        public Task<Currency?> GetCurrencyByIdAsync(int id, CancellationToken ct = default)
            => currencyRepository.GetByIdAsync(id, ct);

        public Task<Currency> CreateCurrencyAsync(Currency model, CancellationToken ct = default)
            => currencyRepository.InsertAsync(model, ct);

        public Task<Currency> UpdateCurrencyAsync(int id, Currency model, CancellationToken ct = default)
            => currencyRepository.UpdateAsync(id, model, ct);

        public Task<bool> DeleteCurrencyAsync(int id, CancellationToken ct = default)
            => currencyRepository.DeleteByIdAsync(id, ct);

        // PriceReadings
        public Task<(IEnumerable<PriceReading> Items, int TotalCount)> GetPriceReadingsPagedAsync(int page, int pageSize, CancellationToken ct = default)
            => priceRepository.GetPagedAsync(page, pageSize, ct);

        public Task<PriceReading?> GetPriceReadingByIdAsync(int id, CancellationToken ct = default)
            => priceRepository.GetByIdAsync(id, ct);

        public Task<PriceReading> CreatePriceReadingAsync(PriceReading model, CancellationToken ct = default)
            => priceRepository.InsertAsync(model, ct);

        public Task<bool> DeletePriceReadingAsync(int id, CancellationToken ct = default)
            => priceRepository.DeleteByIdAsync(id, ct);
    }
}
