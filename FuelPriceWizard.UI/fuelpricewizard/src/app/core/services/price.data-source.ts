import { Observable } from 'rxjs';
import { PriceReadingDto } from '../models/price.model';

/**
 * Abstraction over the (future) price endpoints of the FuelPriceWizard.API.
 * The backend does not expose prices yet, so the mock implementation is the
 * default; once the endpoints exist, flipping `environment.useMockPrices`
 * switches to the HTTP implementation without further UI changes.
 */
export abstract class PriceDataSource {
  /** Latest price reading per fuel type offered by the station. */
  abstract getLatestPrices(gasStationId: number): Observable<PriceReadingDto[]>;

  /** Price readings for one station + fuel type within [from, to]. */
  abstract getPriceHistory(
    gasStationId: number,
    fuelTypeId: number,
    from: Date,
    to: Date,
  ): Observable<PriceReadingDto[]>;
}
