import { Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StationsStore } from '../../../core/services/stations.store';
import { OsmMapComponent, OsmMapMarker } from '../../../shared/osm-map/osm-map.component';
import { CurrentPricesComponent } from '../../prices/current-prices.component';
import { PriceHistoryChartComponent } from '../../prices/price-history-chart.component';
import { OpeningHoursComponent } from './opening-hours.component';

@Component({
  selector: 'app-station-detail',
  standalone: true,
  imports: [
    RouterLink,
    OsmMapComponent,
    OpeningHoursComponent,
    CurrentPricesComponent,
    PriceHistoryChartComponent,
  ],
  templateUrl: './station-detail.component.html',
})
export class StationDetailComponent {
  /** Route parameter, bound via withComponentInputBinding(). */
  public id = input.required<string>();

  protected readonly store = inject(StationsStore);

  protected readonly station = computed(() =>
    this.store.stations().find((station) => station.id === Number(this.id())),
  );

  protected readonly marker = computed<OsmMapMarker | undefined>(() => {
    const station = this.station();
    if (station?.address?.lat == null || station.address.long == null) {
      return undefined;
    }
    return {
      id: station.id,
      content: station.designation,
      lat: station.address.lat,
      long: station.address.long,
    };
  });

  constructor() {
    // Covers deep links: the list view normally populated the store already.
    this.store.load();
  }
}
