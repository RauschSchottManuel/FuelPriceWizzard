using FuelPriceWizard.DataAccess.Entities;
using FuelPriceWizard.DataAccess.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FuelPriceWizard.DataAccess
{
    public class FuelPriceWizardDbContext : DbContext
    {
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }
        public DbSet<GasStation> GasStations { get; set; }
        public DbSet<PriceReading> PriceReadings { get; set; }

        public FuelPriceWizardDbContext(DbContextOptions<FuelPriceWizardDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Currency>(entity =>
            {
                entity.ToTable("Currencies");

                entity.HasKey(c => c.Id).HasName("PK_CurrencyId");

                entity.Property(c => c.Name).HasColumnName("Name").IsRequired();

                entity.Property(c => c.Abbreviation).HasColumnName("Abbreviation").IsRequired();

                entity.Property(c => c.Symbol).HasColumnName("Symbol").IsRequired();
            });

            modelBuilder.Entity<FuelType>(entity =>
            {
                entity.ToTable("FuelTypes");

                entity.HasKey(f => f.Id).HasName("PK_FuelTypeId");

                entity.Property(f => f.DisplayValue).HasColumnName("DisplayValue").IsRequired();

                entity.Property(f => f.Abbreviation).HasColumnName("Abbreviation").IsRequired();

                entity.Property(f => f.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
            });

            modelBuilder.Entity<PriceReading>(entity =>
            {
                entity.ToTable("PriceReadings");

                entity.HasKey(p => p.Id).HasName("PK_PriceReadingId");

                entity.Property(p => p.FetchedAt).HasColumnName("FetchedAt").IsRequired();

                entity.Property(p => p.Value).HasColumnName("Value").HasPrecision(4, 3).IsRequired();

                entity.HasOne(p => p.Currency)
                    .WithOne(c => c.PriceReading)
                    .HasForeignKey<PriceReading>(p => p.CurrencyId);

                entity.HasOne(p => p.FuelType)
                    .WithMany(f => f.PriceReadings)
                    .HasForeignKey(p => p.FuelTypeId);
            });

            modelBuilder.Entity<GasStation>(entity =>
            {
                entity.ToTable("GasStations");

                entity.HasKey(g => g.Id).HasName("PK_GasStationId");

                entity.Property(g => g.Designation).HasColumnName("Designation").IsRequired();

                entity.Property(g => g.IsActive).HasColumnName("IsActive").HasDefaultValue(true);

                entity.Property(g => g.Address).HasConversion(
                    // Convert ProductSpecifications to JSON for the database
                    v => JsonSerializer.Serialize(v, null as JsonSerializerOptions),
                    // Convert JSON from the database to ProductSpecifications
                    v => JsonSerializer.Deserialize<Address>(v, null as JsonSerializerOptions)
                );

                entity.Property(g => g.OpeningHours).HasConversion(
                    v => JsonSerializer.Serialize(v, null as JsonSerializerOptions),
                    v => JsonSerializer.Deserialize<List<OpeningHours>>(v, null as JsonSerializerOptions) ?? new()
                );

                entity.HasMany(g => g.PriceReadings)
                    .WithOne(p => p.GasStation);

                entity.HasMany(g => g.FuelTypes)
                    .WithMany(f => f.GasStations);
            });
        }
    }
}
