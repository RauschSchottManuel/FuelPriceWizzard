import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, computed, inject, input } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { startWith, switchMap } from 'rxjs/operators';
import { FuelTypeDto, GasStationDto } from '../../core/models/gas-station.model';
import { PriceReadingDto } from '../../core/models/price.model';
import { PriceDataSource } from '../../core/services/price.data-source';

interface PriceRow {
  fuelType: FuelTypeDto | undefined;
  reading: PriceReadingDto;
}

@Component({
  selector: 'app-current-prices',
  standalone: true,
  imports: [DatePipe, DecimalPipe],
  templateUrl: './current-prices.component.html',
})
export class CurrentPricesComponent {
  public station = input.required<GasStationDto>();

  private readonly priceDataSource = inject(PriceDataSource);

  /** undefined while (re)loading. */
  private readonly prices = toSignal(
    toObservable(this.station).pipe(
      switchMap((station) =>
        this.priceDataSource.getLatestPrices(station.id).pipe(startWith(undefined)),
      ),
    ),
  );

  protected readonly loading = computed(() => this.prices() === undefined);

  protected readonly rows = computed<PriceRow[]>(() => {
    const prices = this.prices() ?? [];
    const fuelTypes = new Map(this.station().fuelTypes.map((fuelType) => [fuelType.id, fuelType]));
    return prices.map((reading) => ({ fuelType: fuelTypes.get(reading.fuelTypeId), reading }));
  });
}
