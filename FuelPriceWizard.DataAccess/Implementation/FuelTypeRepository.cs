using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using FuelTypeModel = FuelPriceWizard.Domain.Models.FuelType;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class FuelTypeRepository : BaseRepository<FuelType, FuelTypeModel>, IFuelTypeRepository
    {
        protected override Expression<Func<FuelType, object>>[] Includes => [];

        public FuelTypeRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<FuelTypeModel> GetByDisplayValueAsync(string displayValue, CancellationToken ct = default)
        {
            var entity = await this.Context.FuelTypes.FirstOrDefaultAsync(ft => ft.DisplayValue == displayValue, ct);
            return this.Mapper.Map<FuelTypeModel>(entity);
        }
    }
}
