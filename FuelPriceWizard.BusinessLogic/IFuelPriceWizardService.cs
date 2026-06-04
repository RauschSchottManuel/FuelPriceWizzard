using FuelPriceWizard.Domain.Models;

namespace FuelPriceWizard.BusinessLogic
{
    public interface IFuelPriceWizardService
    {
        // GasStations
        Task<(IEnumerable<GasStation> Items, int TotalCount)> GetGasStationsPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<GasStation?> GetGasStationByIdAsync(int id, CancellationToken ct = default);
        Task<GasStation> CreateGasStationAsync(GasStation model, CancellationToken ct = default);
        Task<GasStation> UpdateGasStationAsync(int id, GasStation model, CancellationToken ct = default);
        Task<bool> DeleteGasStationAsync(int id, CancellationToken ct = default);

        // FuelTypes
        Task<(IEnumerable<FuelType> Items, int TotalCount)> GetFuelTypesPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<FuelType?> GetFuelTypeByIdAsync(int id, CancellationToken ct = default);
        Task<FuelType> CreateFuelTypeAsync(FuelType model, CancellationToken ct = default);
        Task<FuelType> UpdateFuelTypeAsync(int id, FuelType model, CancellationToken ct = default);
        Task<bool> DeleteFuelTypeAsync(int id, CancellationToken ct = default);

        // Currencies
        Task<(IEnumerable<Currency> Items, int TotalCount)> GetCurrenciesPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Currency?> GetCurrencyByIdAsync(int id, CancellationToken ct = default);
        Task<Currency> CreateCurrencyAsync(Currency model, CancellationToken ct = default);
        Task<Currency> UpdateCurrencyAsync(int id, Currency model, CancellationToken ct = default);
        Task<bool> DeleteCurrencyAsync(int id, CancellationToken ct = default);

        // PriceReadings
        Task<(IEnumerable<PriceReading> Items, int TotalCount)> GetPriceReadingsPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<PriceReading?> GetPriceReadingByIdAsync(int id, CancellationToken ct = default);
        Task<PriceReading> CreatePriceReadingAsync(PriceReading model, CancellationToken ct = default);
        Task<bool> DeletePriceReadingAsync(int id, CancellationToken ct = default);
    }
}
