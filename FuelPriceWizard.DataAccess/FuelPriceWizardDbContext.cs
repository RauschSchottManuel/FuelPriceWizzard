using Microsoft.EntityFrameworkCore;

namespace FuelPriceWizard.DataAccess
{
    public class FuelPriceWizardDbContext : DbContext
    {
        public FuelPriceWizardDbContext()
        {
            
        }

        public FuelPriceWizardDbContext(DbContextOptions<FuelPriceWizardDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
