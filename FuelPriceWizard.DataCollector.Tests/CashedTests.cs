using FuelPriceWizard.DataAccess.Util;

namespace FuelPriceWizard.DataCollector.Tests
{
    public class CashedTests
    {
        [Fact]
        public void Constructor_FetchesDataOnInit()
        {
            var fetchCount = 0;
            IEnumerable<string> Fetch()
            {
                fetchCount++;
                return ["a", "b"];
            }

            _ = new Cashed<string>(TimeSpan.FromHours(1), Fetch, fetchOnInit: true);

            Assert.Equal(1, fetchCount);
        }

        [Fact]
        public void Constructor_DoesNotFetchData_WhenFetchOnInitIsFalse()
        {
            var fetchCount = 0;
            IEnumerable<string> Fetch()
            {
                fetchCount++;
                return ["a"];
            }

            _ = new Cashed<string>(TimeSpan.FromHours(1), Fetch, fetchOnInit: false);

            Assert.Equal(0, fetchCount);
        }

        [Fact]
        public void Get_ReturnsCachedData_WhenStillValid()
        {
            var fetchCount = 0;
            IEnumerable<string> Fetch()
            {
                fetchCount++;
                return ["cached"];
            }

            var cashed = new Cashed<string>(TimeSpan.FromHours(1), Fetch, fetchOnInit: true);

            // Act: call Get twice with a long TTL
            _ = cashed.Get();
            _ = cashed.Get();

            // Only the initial fetch + no re-fetch on Get calls
            Assert.Equal(1, fetchCount);
        }

        [Fact]
        public void Get_RefetchesData_WhenCacheIsExpired()
        {
            var fetchCount = 0;
            IEnumerable<string> Fetch()
            {
                fetchCount++;
                return ["fresh"];
            }

            // TTL of zero means cache is always expired
            var cashed = new Cashed<string>(TimeSpan.Zero, Fetch, fetchOnInit: false);

            _ = cashed.Get();
            _ = cashed.Get();

            Assert.Equal(2, fetchCount);
        }

        [Fact]
        public void Get_ReturnsDefensiveCopy()
        {
            var cashed = new Cashed<string>(
                TimeSpan.FromHours(1),
                () => ["x", "y"],
                fetchOnInit: true);

            var first = cashed.Get();
            var second = cashed.Get();

            Assert.NotSame(first, second);
        }
    }
}
