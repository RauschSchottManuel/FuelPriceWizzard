# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [Unreleased]

---

## [0.8] — 2025-06-06

### Changed
- DataCollector now reloads only the diff when configuration changes for periodic tasks (PR #8)

---

## [0.7] — 2025-06

### Fixed
- Caching added for `FuelType` and `Currency` lookups to avoid repeated DB queries during collection runs
- Resolved database errors in the DataAccess layer related to fuel type and currency resolution
- Added `Setup()` method to collector base class for initialisation logic (PR #7)

---

## [0.6]

### Added
- Docker support for the DataCollector: `Dockerfile` and `docker-compose.yml` added

### Fixed
- Possible null return in repository resolved
- Update endpoint and API DTOs now include entity IDs
- Code quality improvements and cleanup

---

## [0.5]

### Added
- Gas station REST controller (`GasStationsController`) with full CRUD endpoints
- DTOs (`GasStationDto`, `AddressDto`, `FuelTypeDto`, `OpeningHoursDto`) with AutoMapper profiles
- Structured logging via Serilog added to the API and DataCollector
- `isActive` flag added to gas station model with mapping

---

## [0.4]

### Added
- EControl collector service — fetches fuel prices from the Austrian [E-Control API](https://api.e-control.at/sprit/1.0)
- Currency fetching from the database added to the collector
- Assembly `Enabled` flag in `appsettings.json` to toggle individual collector services
- Fuel type mapping dictionary and base methods for collector services
- Periodic task handling extracted into its own class (`RepeatingTask`)
- Plugin-based collector architecture: assemblies loaded at runtime via `ImplementationAssemblies` config

### Changed
- `BaseFuelPriceSourceService` restructured with improved DI and base class hierarchy
- Improved logging across the DataCollector and collector services

---

## [0.3]

### Added
- Initial Angular 18 UI with OpenStreetMap integration (ngx-leaflet)
- Unit tests for the map component
- SonarQube code quality scan integrated into GitHub Actions CI
- GitHub Actions workflows for .NET (build + test) and Angular (build + lint + test)
- CI badge links added to root README

---

## [0.2]

### Added
- EF Core 8 data access layer with repository pattern (`IGasStationRepository`, `IPriceRepository`, etc.)
- SQL Server / LocalDB connection with EF migrations
- AutoMapper profiles for entity↔model mapping
- Serilog structured logging added to the API

---

## [0.1]

### Added
- Initial project structure: API, BusinessLogic, DataAccess, Domain, DataCollector
- Plugin-based DataCollector with `MockUpFuelPriceSourceCollectorService`
- Basic database schema (gas stations, addresses, price readings, fuel types, currencies)
- Initial Angular UI scaffolding
