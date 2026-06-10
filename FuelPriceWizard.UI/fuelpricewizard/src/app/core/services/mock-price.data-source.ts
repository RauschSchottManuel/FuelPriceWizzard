import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import { PriceReadingDto } from '../models/price.model';
import { GasStationDataSource } from './gas-station.data-source';
import { generatePriceHistory, latestPriceReading } from './mock-price.generator';
import { PriceDataSource } from './price.data-source';

/** Simulated network latency in ms, keeps loading states observable. */
const MOCK_DELAY_MS = 150;

@Injectable()
export class MockPriceDataSource extends PriceDataSource {
  // The station list provides the fuel types per station; getAll() is used
  // because the backend's GetById returns the unmapped domain entity.
  private readonly gasStations = inject(GasStationDataSource);

  override getLatestPrices(gasStationId: number): Observable<PriceReadingDto[]> {
    return this.gasStations.getAll().pipe(
      map((stations) => stations.find((station) => station.id === gasStationId)),
      map((station) =>
        (station?.fuelTypes ?? []).map((fuelType) => latestPriceReading(gasStationId, fuelType.id)),
      ),
    );
  }

  override getPriceHistory(
    gasStationId: number,
    fuelTypeId: number,
    from: Date,
    to: Date,
  ): Observable<PriceReadingDto[]> {
    return of(generatePriceHistory(gasStationId, fuelTypeId, from, to)).pipe(delay(MOCK_DELAY_MS));
  }
}
