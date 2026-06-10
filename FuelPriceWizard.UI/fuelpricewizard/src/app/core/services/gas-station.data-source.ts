import { Observable } from 'rxjs';
import { GasStationDto } from '../models/gas-station.model';

/**
 * Abstraction over the gas-station backend. The abstract class doubles as the
 * DI token; app.config.ts decides between the HTTP and the mock implementation
 * based on the environment.
 */
export abstract class GasStationDataSource {
  abstract getAll(): Observable<GasStationDto[]>;
  abstract getById(id: number): Observable<GasStationDto>;
}
