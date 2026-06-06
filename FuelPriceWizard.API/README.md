# FuelPriceWizard - API

ASP.NET Core 8 REST API that serves gas station data collected by the DataCollector. It reads from the shared SQL Server database and exposes HTTP endpoints consumed by the Angular frontend.

## Endpoints

All routes are prefixed with `/api/gasstations`.

| Method | Route | Description | Status Codes |
|--------|-------|-------------|--------------|
| GET | `/api/gasstations/all` | Returns all gas stations | 200 |
| GET | `/api/gasstations/{id}` | Returns a gas station by ID | 200, 404 |
| POST | `/api/gasstations/new` | Creates a new gas station | 201, 400 |
| PUT | `/api/gasstations/edit/{id}` | Updates an existing gas station | 200, 400 |
| DELETE | `/api/gasstations/delete/{id}` | Deletes a gas station | 204 |

Request and response bodies use the `GasStationDto` schema (JSON), which includes address, fuel types, and opening hours.

## Configuration

Key settings in `appsettings.json`:

| Key | Description |
|-----|-------------|
| `ConnectionStrings:FuelPriceWizard` | SQL Server / LocalDB connection string |
| `Serilog` | Structured logging configuration (console + rolling file) |
| `AllowedHosts` | ASP.NET Core host filtering |

## Running Locally

```bash
dotnet run --project FuelPriceWizard.API
```

The default launch profile (see `Properties/launchSettings.json`) starts the API on `https://localhost:7xxx` / `http://localhost:5xxx`. Swagger UI is available in the Development environment.

## Docker

A multi-stage `Dockerfile` is included. The final image is based on `mcr.microsoft.com/dotnet/aspnet:8.0` and exposes ports **8080** and **8081**.

```bash
docker build -t fuelpricewizard/api .
docker run -p 8080:8080 fuelpricewizard/api
```

The `ConnectionStrings:FuelPriceWizard` environment variable must be set when running in a container.
