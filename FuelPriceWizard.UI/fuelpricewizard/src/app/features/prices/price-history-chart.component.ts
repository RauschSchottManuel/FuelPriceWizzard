import { Component, computed, inject, input, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { ChartConfiguration } from 'chart.js';
import 'chartjs-adapter-date-fns';
import { BaseChartDirective } from 'ng2-charts';
import { filter, startWith, switchMap } from 'rxjs/operators';
import { GasStationDto } from '../../core/models/gas-station.model';
import { PriceDataSource } from '../../core/services/price.data-source';

const DAY_MS = 24 * 60 * 60 * 1000;

@Component({
  selector: 'app-price-history-chart',
  standalone: true,
  imports: [BaseChartDirective],
  templateUrl: './price-history-chart.component.html',
})
export class PriceHistoryChartComponent {
  public station = input.required<GasStationDto>();

  private readonly priceDataSource = inject(PriceDataSource);

  protected readonly rangeOptions = [7, 30, 90] as const;
  protected readonly rangeDays = signal<number>(30);

  private readonly selectedFuelTypeId = signal<number | undefined>(undefined);

  /** Selected fuel type, falling back to the station's first one. */
  protected readonly fuelTypeId = computed<number | undefined>(() => {
    const fuelTypes = this.station().fuelTypes;
    const selected = this.selectedFuelTypeId();
    return fuelTypes.some((fuelType) => fuelType.id === selected)
      ? selected
      : fuelTypes[0]?.id;
  });

  private readonly query = computed(() => ({
    stationId: this.station().id,
    fuelTypeId: this.fuelTypeId(),
    days: this.rangeDays(),
  }));

  /** undefined while (re)loading. */
  private readonly readings = toSignal(
    toObservable(this.query).pipe(
      filter((query) => query.fuelTypeId !== undefined),
      switchMap((query) => {
        const to = new Date();
        const from = new Date(to.getTime() - query.days * DAY_MS);
        return this.priceDataSource
          .getPriceHistory(query.stationId, query.fuelTypeId!, from, to)
          .pipe(startWith(undefined));
      }),
    ),
  );

  protected readonly hasFuelTypes = computed(() => this.station().fuelTypes.length > 0);

  protected readonly loading = computed(() => this.hasFuelTypes() && this.readings() === undefined);

  protected readonly chartData = computed<ChartConfiguration<'line'>['data']>(() => {
    const readings = this.readings() ?? [];
    const fuelType = this.station().fuelTypes.find((ft) => ft.id === this.fuelTypeId());
    return {
      datasets: [
        {
          label: fuelType?.displayValue ?? 'Price',
          data: readings.map((reading) => ({
            x: new Date(reading.fetchedAt).getTime(),
            y: reading.value,
          })),
          borderColor: '#0f766e',
          backgroundColor: 'rgba(15, 118, 110, 0.1)',
          tension: 0.2,
          pointRadius: 0,
          fill: true,
        },
      ],
    };
  });

  protected readonly chartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    scales: {
      x: { type: 'time', time: { unit: 'day' } },
      y: {
        title: { display: true, text: 'EUR / l' },
        ticks: { callback: (value) => Number(value).toFixed(2) },
      },
    },
    plugins: {
      legend: { display: false },
      tooltip: {
        callbacks: {
          label: (context) => (context.parsed.y == null ? '' : ` ${context.parsed.y.toFixed(3)} €`),
        },
      },
    },
  };

  protected selectFuelType(id: number): void {
    this.selectedFuelTypeId.set(id);
  }

  protected selectRange(days: number): void {
    this.rangeDays.set(days);
  }
}
