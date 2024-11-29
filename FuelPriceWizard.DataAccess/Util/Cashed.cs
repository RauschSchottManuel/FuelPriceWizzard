namespace FuelPriceWizard.DataAccess.Util
{
    /// <summary>
    /// Stores an IEnumerable of type T with a validity TimeSpan to ensure cached data is fetched again once it is expired.
    /// </summary>
    /// <typeparam name="T">Type of stored data</typeparam>
    public class Cashed<T>
    {
        private object _lock = new object();
        protected IEnumerable<T> Data { get; set; }
        protected TimeSpan ValidTimeSpan { get; set; }
        protected Func<IEnumerable<T>> FetchData { get; set; }
        public DateTime LastFetched { get; set; }

        public Cashed(TimeSpan validTimeSpan, Func<IEnumerable<T>> fetchAction, bool fetchOnInit = true)
        {
            Data = [];

            ValidTimeSpan = validTimeSpan;
            FetchData = fetchAction;
            if (fetchOnInit)
            {
                ExecuteFetch();
            }
        }

        private void ExecuteFetch()
        {
            Data = FetchData();
            LastFetched = DateTime.Now;
        }

        /// <summary>
        /// Returns the stored data if still valid and fetches to return current data, if expired
        /// </summary>
        /// <returns>Stored data if still valid, current data if stored was expired</returns>
        public IEnumerable<T> Get()
        {
            lock (_lock)
            {
                if (LastFetched + ValidTimeSpan <= DateTime.Now)
                {
                    ExecuteFetch();
                }

                return new List<T>(Data);
            }
        }
    }
}
