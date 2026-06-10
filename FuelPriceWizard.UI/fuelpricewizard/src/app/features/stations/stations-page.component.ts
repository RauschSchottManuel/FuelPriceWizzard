import { Component, computed, inject } from '@angular/core';
import { Router } from '@angular/router';
import { GasStationDto } from '../../core/models/gas-station.model';
import { StationsStore } from '../../core/services/stations.store';
import { OsmMapComponent, OsmMapMarker } from '../../shared/osm-map/osm-map.component';
import { StationListComponent } from './station-list.component';

function escapeHtml(value: string): string {
  return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

@Component({
  selector: 'app-stations-page',
  standalone: true,
  imports: [OsmMapComponent, StationListComponent],
  templateUrl: './stations-page.component.html',
})
export class StationsPageComponent {
  private readonly router = inject(Router);
  protected readonly store = inject(StationsStore);

  /** Stations without coordinates stay in the list but are omitted from the map. */
  protected readonly markers = computed<OsmMapMarker[]>(() =>
    this.store
      .stations()
      .filter((station) => station.address?.lat != null && station.address?.long != null)
      .map((station) => ({
        id: station.id,
        content: `<strong>${escapeHtml(station.designation)}</strong><br>${escapeHtml(
          `${station.address.street}, ${station.address.city}`,
        )}`,
        lat: station.address.lat!,
        long: station.address.long!,
      })),
  );

  constructor() {
    this.store.load();
  }

  protected openStation(station: Pick<GasStationDto, 'id'> | OsmMapMarker): void {
    if (station.id != null) {
      this.router.navigate(['/stations', station.id]);
    }
  }

  protected retry(): void {
    this.store.load(true);
  }
}
