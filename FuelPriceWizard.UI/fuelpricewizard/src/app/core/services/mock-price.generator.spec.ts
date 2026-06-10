import {
  PRICE_SERIES_EPOCH_MS,
  SAMPLE_INTERVAL_MS,
  generatePriceHistory,
  latestPriceReading,
  priceAt,
  sampleIndexFor,
  seriesSeed,
} from './mock-price.generator';

describe('mock-price.generator', () => {
  it('should be deterministic: identical inputs yield identical prices', () => {
    expect(priceAt(1, 2, 100)).toBe(priceAt(1, 2, 100));
    expect(generatePriceHistory(1, 2, new Date('2025-03-01'), new Date('2025-03-08'))).toEqual(
      generatePriceHistory(1, 2, new Date('2025-03-01'), new Date('2025-03-08')),
    );
  });

  it('should produce different series for different stations or fuel types', () => {
    expect(seriesSeed(1, 2)).not.toBe(seriesSeed(2, 2));
    expect(seriesSeed(1, 2)).not.toBe(seriesSeed(1, 3));

    const a = generatePriceHistory(1, 2, new Date('2025-03-01'), new Date('2025-03-08'));
    const b = generatePriceHistory(2, 2, new Date('2025-03-01'), new Date('2025-03-08'));
    expect(a.map((r) => r.value)).not.toEqual(b.map((r) => r.value));
  });

  it('should keep prices within ±10% of the fuel-type base price', () => {
    const basePrices: Record<number, number> = { 1: 1.549, 2: 1.619, 3: 1.789, 4: 1.099 };
    for (const fuelTypeId of [1, 2, 3, 4]) {
      const base = basePrices[fuelTypeId];
      for (let sampleIndex = 0; sampleIndex < 500; sampleIndex++) {
        const value = priceAt(3, fuelTypeId, sampleIndex);
        expect(value).toBeGreaterThan(base * 0.9);
        expect(value).toBeLessThan(base * 1.1);
      }
    }
  });

  it('should round prices to 3 decimals', () => {
    const value = priceAt(1, 1, 42);
    expect(value).toBe(Math.round(value * 1000) / 1000);
  });

  it('should generate one sample per 6-hour slot within the range', () => {
    const from = new Date(PRICE_SERIES_EPOCH_MS + 100 * SAMPLE_INTERVAL_MS);
    const to = new Date(from.getTime() + 7 * 24 * 60 * 60 * 1000);

    const readings = generatePriceHistory(1, 2, from, to);

    expect(readings.length).toBe(29); // 28 intervals + inclusive boundary samples
    expect(readings[0].fetchedAt).toBe(from.toISOString());
    expect(new Date(readings[readings.length - 1].fetchedAt).getTime()).toBeLessThanOrEqual(
      to.getTime(),
    );
  });

  it('should not generate samples before the series epoch', () => {
    const readings = generatePriceHistory(1, 2, new Date('2020-01-01'), new Date(PRICE_SERIES_EPOCH_MS + SAMPLE_INTERVAL_MS));
    expect(readings.length).toBe(2);
    expect(readings[0].fetchedAt).toBe(new Date(PRICE_SERIES_EPOCH_MS).toISOString());
  });

  it('latest reading should equal the last history sample for the same instant', () => {
    const now = new Date(PRICE_SERIES_EPOCH_MS + 1234 * SAMPLE_INTERVAL_MS + 1000);
    const latest = latestPriceReading(2, 1, now);
    const history = generatePriceHistory(2, 1, new Date(now.getTime() - 24 * 60 * 60 * 1000), now);

    expect(latest.value).toBe(history[history.length - 1].value);
    expect(latest.id).toBe(sampleIndexFor(now));
  });
});
