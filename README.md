[![.NET Build & Tests](https://github.com/RauschSchottManuel/FuelPriceWizzard/actions/workflows/dotnet.yml/badge.svg)](https://github.com/RauschSchottManuel/FuelPriceWizzard/actions/workflows/dotnet.yml) [![Angular Build & Tests](https://github.com/RauschSchottManuel/FuelPriceWizzard/actions/workflows/angular.yml/badge.svg)](https://github.com/RauschSchottManuel/FuelPriceWizzard/actions/workflows/angular.yml) [![Quality Gate Status](https://sonarqube.mrausch-schott.com/api/project_badges/measure?project=RauschSchottManuel_FuelPriceWizzard_ee84ae87-553d-4321-b870-ec9b8be6b491&metric=alert_status&token=sqb_a745b528a4aeb4835b34700c9206e76398e6fb30)](https://sonarqube.mrausch-schott.com/dashboard?id=RauschSchottManuel_FuelPriceWizzard_ee84ae87-553d-4321-b870-ec9b8be6b491)
# FuelPriceWizard

> **Note:** This project is still under active development. Documentation is subject to change.

FuelPriceWizard collects fuel prices from external sources and displays them over time on an interactive map. It consists of a plugin-based data collection backend, a REST API, and an Angular frontend.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend API | ASP.NET Core 8 |
| Business Logic | .NET 8 class library |
| Data Access | EF Core 8 + SQL Server / LocalDB |
| Data Collection | .NET 8 worker (plugin-based) |
| Frontend | Angular 18 + TailwindCSS + ngx-leaflet |
| Logging | Serilog |
| Containerisation | Docker / Docker Compose |
| CI/CD | GitHub Actions |
| Code Quality | SonarQube |

## Architecture

```
Angular UI  ──►  FuelPriceWizard.API  ──►  FuelPriceWizard.BusinessLogic
                                                       │
                                        FuelPriceWizard.DataAccess
                                                       │
                                         FuelPriceWizard.Domain (shared models)

FuelPriceWizard.DataCollector  ──►  FuelPriceWizard.BusinessLogic
                                                       │
                                        FuelPriceWizard.DataAccess
```

The **DataCollector** is a standalone worker process that polls external fuel price APIs on a configurable schedule and writes data directly to the database. The **API** serves the stored data to the frontend.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- [Node.js 22+](https://nodejs.org/)
- [Angular CLI 18](https://angular.dev/tools/cli): `npm install -g @angular/cli@18`
- SQL Server or LocalDB (included with Visual Studio)

## Getting Started

```bash
# 1. Clone
git clone https://github.com/RauschSchottManuel/FuelPriceWizzard.git
cd FuelPriceWizzard

# 2. Restore .NET packages
dotnet restore

# 3. Apply database migrations (requires SQL Server / LocalDB)
dotnet ef database update --project FuelPriceWizard.DataAccess --startup-project FuelPriceWizard.API

# 4. Run the API
dotnet run --project FuelPriceWizard.API

# 5. Run the DataCollector
dotnet run --project FuelPriceWizard.DataCollector

# 6. Run the Angular frontend
cd FuelPriceWizard.UI/fuelpricewizard
npm ci
ng serve
# → http://localhost:4200
```

## Sub Projects

### [FuelPriceWizard.API](FuelPriceWizard.API/README.md)
ASP.NET Core 8 REST API that exposes gas station data over HTTP. Provides CRUD endpoints for gas stations.

### [FuelPriceWizard.BusinessLogic](FuelPriceWizard.BusinessLogic/README.md)
Core business logic layer shared between the API and the DataCollector. Defines the interfaces and base classes that collector service plugins must implement.

### [FuelPriceWizard.DataAccess](FuelPriceWizard.DataAccess/README.md)
EF Core 8 data access layer using the repository pattern. Manages all database interactions and migrations.

### [FuelPriceWizard.DataCollector](FuelPriceWizard.DataCollector/README.md)
Plugin-based worker process that collects fuel prices from external APIs on a configurable schedule. New data sources can be added by dropping a `.dll` into the working directory and adding an entry to `appsettings.json`.

### [FuelPriceWizard.Domain](FuelPriceWizard.Domain/README.md)
Shared domain models (e.g. `GasStation`, `PriceReading`, `FuelType`) used across all projects.

### [FuelPriceWizard.UI](FuelPriceWizard.UI/README.md)
Angular 18 frontend that renders gas stations and their price history on an interactive OpenStreetMap.

## Collector Services

Collector service plugins live in the `CollectorServices/` solution folder:

- **EControlCollectorService** — fetches fuel prices from the Austrian [E-Control API](https://api.e-control.at/sprit/1.0)
- **MockUpFuelPriceSourceCollectorService** — mock implementation for local development and testing

See the [DataCollector README](FuelPriceWizard.DataCollector/README.md) for instructions on creating a new collector service.
