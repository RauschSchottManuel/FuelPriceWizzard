using AutoMapper;
using FuelPriceWizard.DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using FuelTypeModel = FuelPriceWizard.Domain.Models.FuelType;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class FuelTypeRepository : BaseRepository<FuelType, FuelTypeModel>, IFuelTypeRepository
    {
        public FuelTypeRepository(FuelPriceWizardDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public override string[] Includes => [];

        public async Task<FuelTypeModel> GetByDisplayValueAsync(string displayValue)
        {
            var entity = await this.Context.FuelTypes.FirstOrDefaultAsync(ft => ft.DisplayValue == displayValue);

            return this.Mapper.Map<FuelTypeModel>(entity);
        }
    }
}
