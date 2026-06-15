# FuelPriceWizard - DataAccess

EF Core 8 data access layer using the repository pattern. Manages all database interactions, entity mappings, and migrations.

## Database

SQL Server or LocalDB (default for local development). The connection string is configured via `appsettings.json`:

```json
"ConnectionStrings": {
  "FuelPriceWizard": "Server=(localdb)\\mssqllocaldb;Database=FuelPriceWizard;..."
}
```

## Repositories

All repositories implement the generic `IRepository<T>` base interface, which provides standard CRUD operations.

| Interface | Implementation | Description |
|-----------|----------------|-------------|
| `IRepository<T>` | `BaseRepository<T>` | Generic CRUD base |
| `IGasStationRepository` | `GasStationRepository` | Gas station queries and persistence |
| `IPriceRepository` | `PriceRepository` | Price reading storage |
| `IFuelTypeRepository` | `FuelTypeRepository` | Fuel type lookup (cached) |
| `ICurrencyRepository` | `CurrencyRepository` | Currency lookup (cached) |

`FuelTypeRepository` and `CurrencyRepository` cache their results after the first database read to avoid repeated lookups during collection runs (see `Util/Cashed.cs`).

## Entity Mapping

AutoMapper profiles in the `Entities/Mapping/` folder map between the `FuelPriceWizard.Domain` models and the EF Core entity classes:

- `GasStationMappingProfile`
- `AddressMappingProfile`
- `OpeningHoursMappingProfile`
- `PriceReadingMappingProfile`
- `FuelTypeMappingProfile`
- `CurrencyMappingProfile`

## Migrations

EF Core migrations are stored in the `Migrations/` folder. To apply all pending migrations:

```bash
dotnet ef database update --project FuelPriceWizard.DataAccess --startup-project FuelPriceWizard.API
```

To add a new migration after changing an entity:

```bash
dotnet ef migrations add <MigrationName> --project FuelPriceWizard.DataAccess --startup-project FuelPriceWizard.API
```

## DI Registration

```csharp
services.AddDataAccessServices(configuration);
```

Call `ServiceRegistrationHelper.AddDataAccessServices()` in your host builder to register the `DbContext`, all repositories, and AutoMapper profiles.
