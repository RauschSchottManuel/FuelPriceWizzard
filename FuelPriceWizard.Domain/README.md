# FuelPriceWizard - Domain

Shared domain model library used across all projects. Contains no business logic — only plain model classes.

## Models

| Model | Description |
|-------|-------------|
| `BaseModel` | Abstract base with a common `Id` property |
| `GasStation` | A gas station with a name, address, opening hours, and associated price readings |
| `Address` | Street, city, country, and geographic coordinates (latitude / longitude) |
| `OpeningHours` | Day-of-week opening and closing times for a gas station |
| `PriceReading` | A recorded fuel price: value, fuel type, currency, timestamp, and a reference to the gas station |
| `FuelType` | A fuel type with a name and code (e.g. Diesel, Super) |
| `Currency` | A currency with a code (e.g. EUR) |

## Usage

This project is referenced by `FuelPriceWizard.BusinessLogic`, `FuelPriceWizard.DataAccess`, and `FuelPriceWizard.API`. Models are mapped to and from EF Core entities in the DataAccess layer using AutoMapper.
