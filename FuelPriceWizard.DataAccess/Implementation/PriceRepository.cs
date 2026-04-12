using AutoMapper;
using FuelPriceWizard.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using PriceModel = FuelPriceWizard.Domain.Models.PriceReading;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class PriceRepository : BaseRepository<PriceReading, PriceModel>, IPriceRepository
    {
        public override string[] Includes => [nameof(PriceReading.Currency), nameof(PriceReading.FuelType)];

        public PriceRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<IEnumerable<PriceModel>> GetLatestByStationAsync(int stationId)
        {
            var entities = await Context.PriceReadings
                .AsNoTracking()
                .Include(p => p.Currency)
                .Include(p => p.FuelType)
                .Where(p => p.GasStationId == stationId)
                .GroupBy(p => p.FuelTypeId)
                .Select(g => g.OrderByDescending(p => p.FetchedAt).First())
                .ToListAsync();

            return Mapper.Map<IEnumerable<PriceModel>>(entities);
        }

        public async Task<IEnumerable<PriceModel>> GetHistoryAsync(int stationId, int fuelTypeId, DateTime from, DateTime to)
        {
            var entities = await Context.PriceReadings
                .AsNoTracking()
                .Include(p => p.Currency)
                .Include(p => p.FuelType)
                .Where(p => p.GasStationId == stationId
                         && p.FuelTypeId == fuelTypeId
                         && p.FetchedAt >= from
                         && p.FetchedAt <= to)
                .OrderBy(p => p.FetchedAt)
                .ToListAsync();

            return Mapper.Map<IEnumerable<PriceModel>>(entities);
        }
    }
}
