import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PriceReadingDto } from '../models/price.model';
import { PriceDataSource } from './price.data-source';

/**
 * Real implementation against the anticipated price endpoints. The routes
 * follow the conventions of the existing GasStationsController
 * (api/[controller] + verb segment); adjust here if the actual API differs.
 */
@Injectable()
export class HttpPriceDataSource extends PriceDataSource {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/PriceReadings`;

  override getLatestPrices(gasStationId: number): Observable<PriceReadingDto[]> {
    return this.http.get<PriceReadingDto[]>(`${this.baseUrl}/latest/${gasStationId}`);
  }

  override getPriceHistory(
    gasStationId: number,
    fuelTypeId: number,
    from: Date,
    to: Date,
  ): Observable<PriceReadingDto[]> {
    const params = new HttpParams()
      .set('from', from.toISOString())
      .set('to', to.toISOString());
    return this.http.get<PriceReadingDto[]>(
      `${this.baseUrl}/history/${gasStationId}/${fuelTypeId}`,
      { params },
    );
  }
}
