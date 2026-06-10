/**
 * Anticipated DTOs for the price endpoints of the FuelPriceWizard.API.
 * The backend domain already has PriceReading and Currency entities, but no
 * controller exposes them yet — these shapes follow the existing DTO
 * conventions so the HTTP data source can go live without UI changes.
 */

export interface CurrencyDto {
  id: number;
  name: string;
  abbreviation: string;
  symbol: string;
}

export interface PriceReadingDto {
  id: number;
  value: number;
  /** ISO 8601 timestamp. */
  fetchedAt: string;
  currency: CurrencyDto;
  fuelTypeId: number;
  gasStationId: number;
}
