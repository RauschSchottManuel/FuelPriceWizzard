import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GasStationDto } from '../models/gas-station.model';
import { GasStationDataSource } from './gas-station.data-source';

@Injectable()
export class HttpGasStationDataSource extends GasStationDataSource {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiBaseUrl}/GasStations`;

  override getAll(): Observable<GasStationDto[]> {
    return this.http.get<GasStationDto[]>(`${this.baseUrl}/all`);
  }

  override getById(id: number): Observable<GasStationDto> {
    return this.http.get<GasStationDto>(`${this.baseUrl}/${id}`);
  }
}
