import { Injectable, Signal, computed, inject, signal } from '@angular/core';
import { GasStationDto } from '../models/gas-station.model';
import { GasStationDataSource } from './gas-station.data-source';

/**
 * Signal-based store for gas stations. Loads once and serves all views from
 * the cached list — including detail lookups, because the backend's GetById
 * endpoint currently returns the unmapped domain entity instead of the DTO.
 */
@Injectable({ providedIn: 'root' })
export class StationsStore {
  private readonly dataSource = inject(GasStationDataSource);

  private readonly stationsState = signal<GasStationDto[]>([]);
  private readonly loadingState = signal(false);
  private readonly errorState = signal<string | undefined>(undefined);
  private loaded = false;

  readonly stations = this.stationsState.asReadonly();
  readonly loading = this.loadingState.asReadonly();
  readonly error = this.errorState.asReadonly();

  /** Idempotent: subsequent calls are no-ops unless `force` or a prior load failed. */
  load(force = false): void {
    if (this.loaded && !force) {
      return;
    }
    this.loaded = true;
    this.loadingState.set(true);
    this.errorState.set(undefined);

    this.dataSource.getAll().subscribe({
      next: (stations) => {
        this.stationsState.set(stations);
        this.loadingState.set(false);
      },
      error: () => {
        this.loaded = false;
        this.loadingState.set(false);
        this.errorState.set('Could not load gas stations. Is the FuelPriceWizard API running?');
      },
    });
  }

  stationById(id: number): Signal<GasStationDto | undefined> {
    return computed(() => this.stationsState().find((station) => station.id === id));
  }
}
