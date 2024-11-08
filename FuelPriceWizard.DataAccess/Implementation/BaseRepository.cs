using AutoMapper;
using FuelPriceWizard.DataAccess.Entities;
using FuelPriceWizard.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public abstract class BaseRepository<TDataModel, TDomainModel> : IRepository<TDomainModel>
        where TDataModel : BaseEntity 
        where TDomainModel : BaseModel
    {
        public FuelPriceWizardDbContext Context { get; set; }
        public IMapper Mapper { get; set; }

        public abstract string[] Includes { get; }

        protected BaseRepository(FuelPriceWizardDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync(params string[] includeItems)
        {
            var query = this.Context.Set<TDataModel>().AsQueryable();

            foreach(var incl in Includes.Union(includeItems).Distinct())
            {
                query = query.Include(incl);
            }

            var entities = await query.ToListAsync();
            return this.Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }

        public async Task<TDomainModel?> GetByIdAsync(int id, params string[] includeItems)
        {
            var entity = await ExecuteGetByIdAsync(id, includeItems);

            return this.Mapper.Map<TDomainModel>(entity);
        }

        public async Task<bool> DeleteAsync(TDomainModel model)
        {
            return await DeleteByIdAsync(model.Id);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var rowsAffected = await this.Context.Set<TDataModel>()
                .Where(e => e.Id == id)
                .ExecuteDeleteAsync();

            await this.Context.SaveChangesAsync();
            
            return rowsAffected > 0;
        }

        public async Task<TDomainModel> UpdateAsync(int id, TDomainModel model)
        {
            var entity = this.Mapper.Map<TDataModel>(model);
            entity.Id = id;

            this.Context.Update(entity);
            await this.Context.SaveChangesAsync();

            return Mapper.Map<TDomainModel>(entity);
        }

        public async Task<TDomainModel> InsertAsync(TDomainModel model)
        {
            var entity = this.Mapper.Map<TDataModel>(model);

            await this.Context.Set<TDataModel>().AddAsync(entity);
            await this.Context.SaveChangesAsync();

            return Mapper.Map<TDomainModel>(entity);
        }

        #region ExecuteMethods

        private async Task<TDataModel> ExecuteGetByIdAsync(int id, params string[] includeItems)
        {
            var query = this.Context.Set<TDataModel>().AsQueryable();

            foreach (var incl in Includes.Union(includeItems))
            {
                query = query.Include(incl);
            }

            return await query.SingleOrDefaultAsync(e => e.Id == id);
        }

        #endregion
    }
}
