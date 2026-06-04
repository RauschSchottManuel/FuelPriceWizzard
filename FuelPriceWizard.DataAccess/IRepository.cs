namespace FuelPriceWizard.DataAccess
{
    public interface IRepository<TDomainModel>
    {
        Task<IEnumerable<TDomainModel>> GetAllAsync(CancellationToken ct = default);
        Task<(IEnumerable<TDomainModel> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct = default);
        Task<TDomainModel?> GetByIdAsync(int id, CancellationToken ct = default);

        Task<bool> DeleteAsync(TDomainModel model, CancellationToken ct = default);
        Task<bool> DeleteByIdAsync(int id, CancellationToken ct = default);

        /// <exception cref="FuelPriceWizard.DataAccess.Exceptions.NotFoundException">Thrown when no entity with the given id exists.</exception>
        Task<TDomainModel> UpdateAsync(int id, TDomainModel model, CancellationToken ct = default);

        Task<TDomainModel> InsertAsync(TDomainModel model, CancellationToken ct = default);
    }
}
