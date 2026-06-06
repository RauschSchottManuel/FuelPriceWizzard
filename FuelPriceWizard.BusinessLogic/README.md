# FuelPriceWizard - BusinessLogic

Core business logic layer shared between the API and the DataCollector. It defines the abstractions that collector service plugins must implement and provides the base infrastructure for interacting with the database and configuration.

## Key Abstractions

### `IFuelPriceSourceService`
Interface that every collector service plugin must implement. Defines the contract for fetching price readings by location and fuel type.

### `BaseFuelPriceSourceService<T>`
Abstract base class for collector implementations. Inject-ready: receives `IConfiguration`, `ILogger<T>`, `IFuelTypeRepository`, and `ICurrencyRepository` via the constructor. Extend this class when creating a new collector service plugin.

### `IFuelPriceWizardService` / `FuelPriceWizardService`
Main application service interface and implementation used by the API layer. Currently a stub — in active development.

### `ServiceRegistrationHelper`
Extension methods to register all BusinessLogic services with the .NET DI container (`IServiceCollection`).

## Enums

Located in `Modules/Enums/`:

| Enum | Values |
|------|--------|
| `FuelType` | `Diesel`, `Super`, and others |
| `Currency` | `EUR`, and others |

## Adding a New Collector Service

A new collector service must:
1. Reference this project (`FuelPriceWizard.BusinessLogic`)
2. Create a class that extends `BaseFuelPriceSourceService<T>` and implements `IFuelPriceSourceService`
3. Override `FuelTypeMapping` (maps source-specific codes to `FuelType` enum values) and `Currency`
4. Implement the `FetchPricesByLocationAsync` and `FetchPricesByLocationAndFuelTypeAsync` methods

See the [DataCollector README](../FuelPriceWizard.DataCollector/README.md) for a step-by-step guide with a full example.
