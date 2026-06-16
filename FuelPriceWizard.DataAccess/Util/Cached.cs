namespace FuelPriceWizard.DataAccess.Util
{
    /// <summary>
    /// Stores an IEnumerable of type T with a validity TimeSpan to refresh cached data once it expires.
    /// Data is fetched lazily on the first call to <see cref="GetAsync"/> and re-fetched when the TTL elapses.
    /// </summary>
    public class Cached<T>
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private IEnumerable<T> _data = [];
        private readonly TimeSpan _validTimeSpan;
        private readonly Func<Task<IEnumerable<T>>> _fetchData;
        private DateTime _lastFetched = DateTime.MinValue;

        public Cached(TimeSpan validTimeSpan, Func<Task<IEnumerable<T>>> fetchAction)
        {
            _validTimeSpan = validTimeSpan;
            _fetchData = fetchAction;
        }

        /// <summary>
        /// Returns the cached data if still valid; otherwise re-fetches from the source.
        /// </summary>
        public async Task<IEnumerable<T>> GetAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_lastFetched + _validTimeSpan <= DateTime.UtcNow)
                {
                    _data = await _fetchData();
                    _lastFetched = DateTime.UtcNow;
                }
                return new List<T>(_data);
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
