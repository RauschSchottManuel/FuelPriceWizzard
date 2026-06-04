# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

---

## Core principle

**If unsure or missing information, ask instead of assuming.** A clarifying question costs seconds; a wrong assumption costs rework. This applies to every agent and every task — requirements, design, implementation, and review alike.

---

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

# Add a new EF Core migration
dotnet ef migrations add <MigrationName> --project FuelPriceWizard.DataAccess --startup-project FuelPriceWizard.API
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

### Solution folder structure

```
FuelPriceWizard.sln
├── FuelPriceWizard.Domain
├── FuelPriceWizard.DataAccess
├── FuelPriceWizard.BusinessLogic
├── FuelPriceWizard.API
├── FuelPriceWizard.DataCollector
├── CollectorServices/
│   ├── EControlCollectorService          ← production collector plugin
│   └── MockUpFuelPriceSourceCollectorService  ← demo/mock plugin
└── Tests/
    ├── FuelPriceWizard.DataCollector.Tests
    ├── FuelPriceWizard.API.Tests
    ├── EControlCollectorService.Tests
    └── FuelPriceWizard.IntegrationTests
```

### Data collector plugin system

`DataCollectorOrchestrator` scans `appsettings.json` for `ImplementationAssemblies` entries, loads each enabled `.dll` via reflection, and wraps each discovered `IFuelPriceSourceService` implementation in a `RepeatingTask<T>`. The orchestrator watches `appsettings.json` for live config changes and reloads only the tasks whose settings changed.

To write a new collector, implement `BaseFuelPriceSourceService<TSettings>` from `FuelPriceWizard.BusinessLogic` and deploy the `.dll` into the collector's plugin directory. See `FuelPriceWizard.DataCollector/README.md` for the full walkthrough.

**Required interface methods:**

```csharp
IConfigurationSection GetFetchSettingsSection();
Task Setup();
Task<IEnumerable<PriceReading>> FetchPricesByLocationAsync(decimal lat, decimal lon, bool includeClosed = true);
Task<IEnumerable<PriceReading>> FetchPricesByLocationAndFuelTypeAsync(decimal lat, decimal lon, FuelType fuelType, bool includeClosed = true);
```

**`BaseFuelPriceSourceService<T>` provides:**
- Constructor-injected: `IConfiguration`, `ILogger<T>`, `IFuelTypeRepository`, `ICurrencyRepository`
- `FuelTypeMapping` (abstract) — maps source-specific strings to domain `FuelType`
- `CachedFuelTypes` / `CachedCurrencies` — 2-hour TTL in-memory cache, avoids redundant DB calls
- `MapToFuelTypeAsync()` / `MapFromFuelType()` helper methods

### Data flow

1. Each running collector calls `FetchPricesByLocationAsync()` on its configured schedule.
2. The orchestrator queries active `GasStation` records from DB, then runs the collector for each station in parallel.
3. Results are persisted via `IPriceRepository.InsertAsync()` inside the collector's own DI scope (each plugin owns its own `DbContext`).
4. The API exposes the stored data through five controllers: `AuthController`, `GasStationsController`, `PriceReadingsController`, `FuelTypesController`, `CurrenciesController`.
5. Angular queries the API and renders gas stations on an interactive Leaflet map.

### Key cross-cutting details

- **Database**: SQL Server LocalDB for development. Connection string: `Server=(localdb)\MSSqlLocalDB;Database=FuelPriceWizard;TrustServerCertificate=true;`
- **Caching**: `Cached<T>` in `DataAccess/Util/Cached.cs` — async TTL-based in-memory cache backed by `SemaphoreSlim`, used for fuel types and currencies.
- **Logging**: Serilog, writing to console and rolling file. Configured under `Serilog` in each project's `appsettings.json`.
- **Mapping**: AutoMapper 13 between domain models and DTOs in the API. Profiles live in `FuelPriceWizard.API/Mapping/`.
- **Address / OpeningHours** are serialised as JSON blobs in the DB — they are not queryable columns.

---

## API reference

### Authentication

The API uses JWT bearer tokens. Obtain a token via:

```
POST /api/auth/token
Body: { "password": "<AdminPassword from appsettings>" }
Returns: { "token": "<JWT>" }
```

Include `Authorization: Bearer <token>` on all mutating requests. Read endpoints are public.

### Endpoints

All list endpoints return a `PagedResult<T>` with `page` and `pageSize` query parameters (default `pageSize=20`).

| Controller | Route | Public GETs | Protected writes |
|---|---|---|---|
| `AuthController` | `api/auth` | — | POST /token |
| `GasStationsController` | `api/gasstations` | GET, GET /{id} | POST, PUT /{id}, DELETE /{id} |
| `PriceReadingsController` | `api/pricereadings` | GET, GET /{id} | POST, DELETE /{id} |
| `FuelTypesController` | `api/fueltypes` | GET, GET /{id} | POST, PUT /{id}, DELETE /{id} |
| `CurrenciesController` | `api/currencies` | GET, GET /{id} | POST, PUT /{id}, DELETE /{id} |

Swagger UI is available at `/swagger` when running the API.

---

## Configuration

### API — `appsettings.json` top-level keys

```json
{
  "ConnectionStrings": { "FuelPriceWizard": "..." },
  "JwtSettings": {
    "Secret": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpirationMinutes": "60",
    "AdminPassword": "..."
  },
  "Serilog": { ... },
  "AllowedHosts": "*"
}
```

### DataCollector — `appsettings.json` top-level keys

```json
{
  "ConnectionStrings": { "FuelPriceWizard": "..." },
  "ImplementationAssemblies": [
    {
      "Enabled": true,
      "FilePath": "path/to/CollectorService.dll",
      "Type": "Namespace.ClassName"
    }
  ],
  "Serilog": { ... }
}
```

### Collector plugin — `FetchSettings` shape (per-plugin appsettings file)

```json
{
  "FetchSettings": {
    "IntervalValue": 5,
    "IntervalUnit": "Second | Minute | Hour",
    "StartNextFullHour": false,
    "ExcludedWeekdays": ["Saturday", "Sunday"]
  }
}
```

---

## Database migrations

Migrations live in `FuelPriceWizard.DataAccess/Migrations/`. Always use both `--project` and `--startup-project` flags — the startup project supplies the connection string.

When adding a migration, verify the generated `.cs` file before applying. The `FuelPriceWizardDbContextModelSnapshot.cs` is auto-maintained and should not be edited manually.

---

## Testing

| Project | What it tests |
|---|---|
| `FuelPriceWizard.DataCollector.Tests` | `DataCollectorOrchestrator` lifecycle, `RepeatingTask<T>` scheduling |
| `FuelPriceWizard.API.Tests` | Auth controller (JWT), GasStations controller, AutoMapper profiles |
| `EControlCollectorService.Tests` | EControl collector-specific logic |
| `FuelPriceWizard.IntegrationTests` | End-to-end integration tests |

Run with coverage (matches CI):

```powershell
dotnet test FuelPriceWizard.sln --collect:"XPlat Code Coverage" --results-directory ./coverage
```

---

## CI / GitHub Actions

| Workflow | Trigger | Jobs |
|---|---|---|
| `dotnet.yml` | Push/PR to `main` on .NET paths | `build` (Release), `test` (with opencover coverage) |
| `angular.yml` | Push/PR to `main` on UI paths | `build` (lint + build), `test` (ChromeHeadless + coverage) |
| `sonarqube.yml` | Push/PR to `main` | SonarQube quality gate |

CI runs on Ubuntu. Artifacts (build output, coverage reports, test results) are uploaded per run.

---

## Development workflow (agents)

Seven subagents live in `.claude/agents/` and cover the full delivery pipeline. Address them by name in your prompt or use `--agent <name>`.

### Agent roster

| Agent | Model | Role |
|---|---|---|
| `product-owner` | Sonnet | Requirements, PRD, bug tickets, acceptance criteria |
| `architect` | Opus | ADRs, component design, data model decisions |
| `developer` | Opus | Implementation, unit tests, `/run` + `/simplify` |
| `code-reviewer` | Sonnet | Code quality, idioms, layer violations — uses `/code-review` |
| `security-expert` | Opus | Injection, auth, data exposure, CVEs — uses `/security-review` |
| `tester` | Sonnet | Acceptance criteria, regressions — uses `/verify` + `/run` |
| `docs-writer` | Sonnet | README, CHANGELOG, CLAUDE.md, ADR acceptance |

### Pipeline

```
User idea / bug report
    │
    ▼
[product-owner] → PRD + AC-N + task list
    │
    ├─ (if new storage / integration / service boundary / API surface)
    │       ▼
    │   [architect] → ADR + component diagram + migration plan
    │
    ▼
[developer] → implementation + tests + /run + /simplify
    │
    ▼ (fan out in parallel)
[code-reviewer]   [security-expert]*   [tester]
    │                   │                  │
    └───────────────────┴──────────────────┘
                        ▼
               Merge parallel verdicts
                        │
                        ▼ (if all APPROVED / APPROVED WITH CONDITIONS)
                [docs-writer] → CHANGELOG + README + ADR-Accepted
```

`*` Skip `security-expert` for purely internal tooling with no user-facing surface and no data persistence.

### Handoff signals

| Signal | Emitted by | Meaning |
|---|---|---|
| `READY FOR ARCHITECT` | product-owner | PRD done; design needed before coding |
| `READY FOR DEVELOPER` | product-owner, architect | Spec/ADR done; coding can start |
| `READY FOR TESTER` | developer, code-reviewer, security-expert | Pass complete; verification can proceed |
| `APPROVED` | tester | Shippable as-is |
| `APPROVED WITH CONDITIONS` | code-reviewer, security-expert, tester | Shippable; non-blocking follow-ups listed |
| `BACK TO PRODUCT-OWNER` | architect, developer, docs-writer | Spec gap; product decision required |
| `BACK TO ARCHITECT` | developer | Design issue uncovered during implementation |
| `BACK TO DEVELOPER` | code-reviewer, security-expert, tester | Findings must be fixed before proceeding |
| `DOCS READY` | docs-writer | Documentation complete |
| `BLOCKED` | any | Cannot proceed; human intervention required |

### Skills wired into agents

| Skill | Used by | When |
|---|---|---|
| `/run` | developer, tester | After implementation / during verification |
| `/simplify` | developer | After implementation, before handing to reviewer |
| `/code-review` | code-reviewer | At start of every review pass |
| `/security-review` | security-expert | At start of every security review pass |
| `/verify` | tester | To confirm the specific change behaves correctly |

---

## Tech stack

| Concern | Technology |
|---|---|
| Backend language | C# 12 / .NET 8 |
| Web framework | ASP.NET Core 8 |
| ORM | EF Core 8 + SQL Server |
| Object mapping | AutoMapper 13 |
| Logging | Serilog 4 |
| Authentication | JWT bearer (Microsoft.AspNetCore.Authentication.JwtBearer 8) |
| API docs | Swashbuckle / Swagger 6.7 |
| Unit testing | xUnit + Moq |
| Frontend | Angular 18, TypeScript 5.5 |
| Styling | Tailwind CSS 3.4 |
| Maps | Leaflet via @asymmetrik/ngx-leaflet |
| Frontend testing | Karma + Jasmine |
| Linting | ESLint 9 + angular-eslint |
| CI | GitHub Actions (dotnet.yml, angular.yml, sonarqube.yml) |
