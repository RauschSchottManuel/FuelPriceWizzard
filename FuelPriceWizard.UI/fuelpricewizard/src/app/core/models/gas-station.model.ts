/**
 * Mirrors of the FuelPriceWizard.API DTOs (System.Text.Json camelCase).
 */

export interface AddressDto {
  id: number;
  street: string;
  zip: string;
  city: string;
  country: string;
  lat?: number | null;
  long?: number | null;
}

export interface FuelTypeDto {
  id: number;
  displayValue: string;
  abbreviation: string;
}

/** .NET DayOfWeek: 0 = Sunday … 6 = Saturday. */
export interface OpeningHoursDto {
  id: number;
  day: number;
  /** .NET TimeOnly, serialized as "HH:mm:ss". */
  from: string;
  to: string;
}

export interface GasStationDto {
  id: number;
  designation: string;
  address: AddressDto;
  fuelTypes: FuelTypeDto[];
  openingHours: OpeningHoursDto[];
}
