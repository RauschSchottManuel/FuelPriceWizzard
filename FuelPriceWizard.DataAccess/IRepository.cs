namespace FuelPriceWizard.DataAccess
{
    public interface IRepository<TDomainModel>
    {
        public Task<IEnumerable<TDomainModel>> GetAllAsync(params string[] includeItems);
        public Task<TDomainModel?> GetByIdAsync(int id, params string[] includeItems);

        public Task<bool> DeleteAsync(TDomainModel model);
        public Task<bool> DeleteByIdAsync(int id);

        public Task<TDomainModel> UpdateAsync(int id, TDomainModel model);

        public Task<TDomainModel> InsertAsync(TDomainModel model);
    }
}
