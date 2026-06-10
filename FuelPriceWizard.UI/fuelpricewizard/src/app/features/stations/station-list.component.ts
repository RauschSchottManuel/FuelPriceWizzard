import { Component, computed, input, output, signal } from '@angular/core';
import { GasStationDto } from '../../core/models/gas-station.model';

@Component({
  selector: 'app-station-list',
  standalone: true,
  templateUrl: './station-list.component.html',
})
export class StationListComponent {
  public stations = input.required<GasStationDto[]>();
  public stationSelected = output<GasStationDto>();

  protected readonly filter = signal('');

  protected readonly filteredStations = computed<GasStationDto[]>(() => {
    const term = this.filter().trim().toLowerCase();
    const stations = this.stations();
    if (!term) {
      return stations;
    }
    return stations.filter(
      (station) =>
        station.designation.toLowerCase().includes(term) ||
        station.address?.city?.toLowerCase().includes(term),
    );
  });

  protected onSearch(event: Event): void {
    this.filter.set((event.target as HTMLInputElement).value);
  }
}
