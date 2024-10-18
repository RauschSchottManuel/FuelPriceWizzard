namespace FuelPriceWizard.DataAccess
{
    public interface IRepository<TDomainModel>
    {
        public Task<IEnumerable<TDomainModel>> GetAllAsync();
        public Task<TDomainModel?> GetByIdAsync(int id);

        public Task<bool> DeleteAsync(TDomainModel model);
        public Task<bool> DeleteByIdAsync(int id);

        public Task<TDomainModel> UpdateAsync(TDomainModel model);

        public Task<TDomainModel> InsertAsync(TDomainModel model);
    }
}
