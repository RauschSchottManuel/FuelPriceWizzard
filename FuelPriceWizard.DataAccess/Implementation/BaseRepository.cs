using AutoMapper;
using FuelPriceWizard.DataAccess.Entities;
using FuelPriceWizard.DataAccess.Exceptions;
using FuelPriceWizard.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FuelPriceWizard.DataAccess.Implementation
{
    public abstract class BaseRepository<TDataModel, TDomainModel> : IRepository<TDomainModel>
        where TDataModel : BaseEntity
        where TDomainModel : BaseModel
    {
        protected FuelPriceWizardDbContext Context { get; }
        protected IMapper Mapper { get; }

        protected abstract Expression<Func<TDataModel, object>>[] Includes { get; }

        protected BaseRepository(FuelPriceWizardDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public async Task<IEnumerable<TDomainModel>> GetAllAsync(CancellationToken ct = default)
        {
            var query = this.Context.Set<TDataModel>().AsNoTracking().AsQueryable();
            foreach (var incl in Includes)
            {
                query = query.Include(incl);
            }
            var entities = await query.ToListAsync(ct);
            return this.Mapper.Map<IEnumerable<TDomainModel>>(entities);
        }

        public async Task<(IEnumerable<TDomainModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            var query = this.Context.Set<TDataModel>().AsNoTracking().AsQueryable();
            foreach (var incl in Includes)
                query = query.Include(incl);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (Mapper.Map<IEnumerable<TDomainModel>>(items), totalCount);
        }

        public async Task<TDomainModel?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await ExecuteGetByIdAsync(id, ct);
            return this.Mapper.Map<TDomainModel>(entity);
        }

        public async Task<bool> DeleteAsync(TDomainModel model, CancellationToken ct = default)
        {
            return await DeleteByIdAsync(model.Id, ct);
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken ct = default)
        {
            var rowsAffected = await this.Context.Set<TDataModel>()
                .Where(e => e.Id == id)
                .ExecuteDeleteAsync(ct);
            return rowsAffected > 0;
        }

        public async Task<TDomainModel> UpdateAsync(int id, TDomainModel model, CancellationToken ct = default)
        {
            var exists = await this.Context.Set<TDataModel>().AnyAsync(e => e.Id == id, ct);
            if (!exists)
                throw new NotFoundException($"{typeof(TDomainModel).Name} with id {id} was not found.");

            var entity = this.Mapper.Map<TDataModel>(model);
            entity.Id = id;
            this.Context.Update(entity);
            await this.Context.SaveChangesAsync(ct);
            return Mapper.Map<TDomainModel>(entity);
        }

        public async Task<TDomainModel> InsertAsync(TDomainModel model, CancellationToken ct = default)
        {
            var entity = this.Mapper.Map<TDataModel>(model);
            await this.Context.Set<TDataModel>().AddAsync(entity, ct);
            await this.Context.SaveChangesAsync(ct);
            return Mapper.Map<TDomainModel>(entity);
        }

        private async Task<TDataModel?> ExecuteGetByIdAsync(int id, CancellationToken ct = default)
        {
            var query = this.Context.Set<TDataModel>().AsNoTracking().AsQueryable();
            foreach (var incl in Includes)
            {
                query = query.Include(incl);
            }
            return await query.SingleOrDefaultAsync(e => e.Id == id, ct);
        }
    }
}
