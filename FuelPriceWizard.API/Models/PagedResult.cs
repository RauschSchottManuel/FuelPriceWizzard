namespace FuelPriceWizard.API.Models
{
    /// <summary>
    /// A paginated response wrapper.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    }
}
