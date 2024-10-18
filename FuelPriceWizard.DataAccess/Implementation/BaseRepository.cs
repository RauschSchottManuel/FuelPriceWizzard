using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public class BaseRepository<TDataModel, TDomainModel> : IRepository<TDomainModel> where TDataModel : class
    {
        public FuelPriceWizardDbContext Context { get; set; }
        public IMapper Mapper { get; set; }


        public BaseRepository(FuelPriceWizardDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync()
        {
            var entities = await this.Context.Set<TDataModel>().ToListAsync();

            return this.Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }
    }
}
