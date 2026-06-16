# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

FuelPriceWizzard is a full-stack application for collecting and displaying fuel prices from Austrian gas stations. It consists of an ASP.NET Core 8 REST API, an Angular 18 SPA, a plugin-based data-collector console app, and a SQL Server database accessed through EF Core.

---

## Commands

### .NET backend

```powershell
# Build entire solution
dotnet build FuelPriceWizard.sln

# Run all tests
dotnet test FuelPriceWizard.sln

# Run a single test by name
dotnet test FuelPriceWizard.DataCollector.Tests --filter "DataCollectorOrchestrator_ShouldBeInstantiatedSuccessfully"

# Run the API
dotnet run --project FuelPriceWizard.API

# Run the data collector
dotnet run --project FuelPriceWizard.DataCollector

# Apply EF Core migrations
dotnet ef database update --project FuelPriceWizard.DataAccess --startup-project FuelPriceWizard.API
```

### Angular frontend

```powershell
cd FuelPriceWizard.UI/fuelpricewizard

npm ci                        # install dependencies
npm start                     # dev server (proxies /api to the .NET API)
npm run build                 # production build
npm test                      # run Karma tests
npm run lint                  # ESLint check
```

---

## Architecture

### Layer hierarchy (no upward dependencies)

```
FuelPriceWizard.Domain          ← pure models, no dependencies
        ↑
FuelPriceWizard.DataAccess      ← EF Core + repository pattern
        ↑
FuelPriceWizard.BusinessLogic   ← base classes for collector plugins
        ↑
FuelPriceWizard.API             ← ASP.NET Core REST API
FuelPriceWizard.DataCollector   ← console app, loads collector plugins
```

### Data collector plugin system

`DataCollectorOrchestrator` scans a configured folder for `.dll` files, loads them via reflection, and wraps each discovered `IFuelPriceSourceService` implementation in a `RepeatingTask<T>`. The orchestrator watches `appsettings.json` for live config changes and reloads only the tasks whose settings changed.

To write a new collector, implement `BaseFuelPriceSourceService<TSettings>` from `FuelPriceWizard.BusinessLogic` and deploy the `.dll` into the collector's plugin directory. The DataCollector README has the full walkthrough.

### Data flow

1. Each running collector calls `FetchPricesByLocationAsync()` on its schedule.
2. Results are persisted via `IPriceRepository.InsertAsync()` inside the collector's own DI scope (each plugin owns its own `DbContext`).
3. The API exposes the stored data through four controllers: `GasStationsController`, `PriceReadingsController`, `FuelTypesController`, `CurrenciesController`.
4. Angular queries the API and renders gas stations on an interactive Leaflet map.

### Key cross-cutting details

- **Database**: SQL Server LocalDB for development. Connection string is `Server=(localdb)\MSSqlLocalDB;Database=FuelPriceWizard;TrustServerCertificate=true;`.
- **Caching**: `Cached<T>` in `DataAccess/Util/Cached.cs` is an async TTL-based in-memory cache wrapper (backed by `SemaphoreSlim`) used for fuel types and currencies to avoid redundant DB round-trips.
- **Logging**: Serilog, writing to console and rolling file. Configured in `appsettings.json` under `Serilog`.
- **Mapping**: AutoMapper is used between domain models and DTOs in the API.
- **Address / OpeningHours** are serialised as JSON blobs in the DB — they are not queryable columns. Keep this in mind if adding geo-search features.

---

## Tech stack

| Concern | Technology |
|---|---|
| Backend language | C# 12 / .NET 8 |
| Web framework | ASP.NET Core 8 |
| ORM | EF Core 8 + SQL Server |
| Object mapping | AutoMapper 13 |
| Logging | Serilog 4 |
| Unit testing | xUnit 2.9 + Moq 4.20 |
| Frontend | Angular 18, TypeScript 5.5 |
| Styling | Tailwind CSS 3.4 |
| Maps | Leaflet via @asymmetrik/ngx-leaflet |
| Frontend testing | Karma + Jasmine |
| Linting | ESLint 9 + angular-eslint |
| CI | GitHub Actions (dotnet.yml, angular.yml, sonarqube.yml) |

---

## Known issues and planned improvements

`ImprovementPlan.md` at the repository root is the authoritative issue tracker. It lists 80+ items organised by project and severity (🔴 critical → 🟢 low). Check it before starting any significant work — many areas have pre-identified problems.

Critical areas highlighted there:
- Threading / concurrency issues in `DataCollectorOrchestrator`
- No tests in `FuelPriceWizard.API.Tests` (the project exists but is empty)
- Non-standard REST routes (e.g. `/api/gasstations/delete/{id}` should be `DELETE /api/gasstations/{id}`)
- No CORS configuration in the API
- No authentication or authorisation anywhere
