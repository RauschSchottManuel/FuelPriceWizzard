import { CurrencyDto, PriceReadingDto } from '../models/price.model';

/**
 * Deterministic fuel-price generator backing the mock price data source.
 *
 * The series is sampled on a fixed grid (every 6 hours since a fixed epoch)
 * and every sample is computed statelessly from a per-series seed, so the
 * same station/fuel-type/time always yields the identical price — across
 * page reloads and in tests.
 */

export const PRICE_SERIES_EPOCH_MS = Date.UTC(2025, 0, 1);
export const SAMPLE_INTERVAL_MS = 6 * 60 * 60 * 1000;

/** EUR per litre; keys match the FuelType ids used by the mock stations. */
const FUEL_TYPE_BASE_PRICES: Record<number, number> = {
  1: 1.549, // Diesel
  2: 1.619, // Super 95
  3: 1.789, // Super Plus 98
  4: 1.099, // CNG
};
const FALLBACK_BASE_PRICE = 1.5;

export const MOCK_CURRENCY: CurrencyDto = {
  id: 1,
  name: 'Euro',
  abbreviation: 'EUR',
  symbol: '€',
};

export function seriesSeed(gasStationId: number, fuelTypeId: number): number {
  return (Math.imul(gasStationId, 2654435761) ^ Math.imul(fuelTypeId, 40503)) >>> 0;
}

/** Stateless hash-PRNG (mulberry32 single step) → [0, 1). */
function hashNoise(seed: number, sampleIndex: number): number {
  let t = ((seed ^ Math.imul(sampleIndex, 0x9e3779b9)) + 0x6d2b79f5) >>> 0;
  t = Math.imul(t ^ (t >>> 15), t | 1);
  t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
  return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}

/** Average of the three surrounding samples, so the curve wiggles instead of crackling. */
function smoothedNoise(seed: number, sampleIndex: number): number {
  return (
    (hashNoise(seed, sampleIndex - 1) + hashNoise(seed, sampleIndex) + hashNoise(seed, sampleIndex + 1)) / 3
  );
}

export function sampleIndexFor(date: Date): number {
  return Math.floor((date.getTime() - PRICE_SERIES_EPOCH_MS) / SAMPLE_INTERVAL_MS);
}

export function sampleTimestamp(sampleIndex: number): Date {
  return new Date(PRICE_SERIES_EPOCH_MS + sampleIndex * SAMPLE_INTERVAL_MS);
}

export function priceAt(gasStationId: number, fuelTypeId: number, sampleIndex: number): number {
  const base = FUEL_TYPE_BASE_PRICES[fuelTypeId] ?? FALLBACK_BASE_PRICE;
  const seed = seriesSeed(gasStationId, fuelTypeId);
  // ~4-week macro cycle (112 samples) + within-day cycle (4 samples) + smoothed jitter.
  const macro = 0.05 * Math.sin((2 * Math.PI * sampleIndex) / 112);
  const daily = 0.015 * Math.sin((2 * Math.PI * sampleIndex) / 4);
  const jitter = (smoothedNoise(seed, sampleIndex) - 0.5) * 0.06;
  // Fuel prices are quoted in tenths of a cent → 3 decimals.
  return Math.round(base * (1 + macro + daily + jitter) * 1000) / 1000;
}

function readingAt(gasStationId: number, fuelTypeId: number, sampleIndex: number): PriceReadingDto {
  return {
    id: sampleIndex,
    value: priceAt(gasStationId, fuelTypeId, sampleIndex),
    fetchedAt: sampleTimestamp(sampleIndex).toISOString(),
    currency: MOCK_CURRENCY,
    fuelTypeId,
    gasStationId,
  };
}

export function generatePriceHistory(
  gasStationId: number,
  fuelTypeId: number,
  from: Date,
  to: Date,
): PriceReadingDto[] {
  const firstIndex = Math.max(0, Math.ceil((from.getTime() - PRICE_SERIES_EPOCH_MS) / SAMPLE_INTERVAL_MS));
  const lastIndex = sampleIndexFor(to);

  const readings: PriceReadingDto[] = [];
  for (let index = firstIndex; index <= lastIndex; index++) {
    readings.push(readingAt(gasStationId, fuelTypeId, index));
  }
  return readings;
}

export function latestPriceReading(gasStationId: number, fuelTypeId: number, now = new Date()): PriceReadingDto {
  return readingAt(gasStationId, fuelTypeId, sampleIndexFor(now));
}
