namespace FuelPriceWizard.DataAccess
{
    public interface IRepository<TDomainModel>
    {
        public Task<IEnumerable<TDomainModel>> GetAllAsync();
        //TODO: CRUD
    }
}
