import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { delay } from 'rxjs/operators';
import { FuelTypeDto, GasStationDto, OpeningHoursDto } from '../models/gas-station.model';
import { GasStationDataSource } from './gas-station.data-source';

export const MOCK_FUEL_TYPES: Record<string, FuelTypeDto> = {
  diesel: { id: 1, displayValue: 'Diesel', abbreviation: 'DIE' },
  super95: { id: 2, displayValue: 'Super 95', abbreviation: 'SUP' },
  superPlus98: { id: 3, displayValue: 'Super Plus 98', abbreviation: 'SUP+' },
  cng: { id: 4, displayValue: 'CNG', abbreviation: 'CNG' },
};

function weekdays(idStart: number, from: string, to: string, days: number[]): OpeningHoursDto[] {
  return days.map((day, i) => ({ id: idStart + i, day, from, to }));
}

export const MOCK_GAS_STATIONS: GasStationDto[] = [
  {
    id: 1,
    designation: 'Turmöl Linz Hauptbahnhof',
    address: {
      id: 1,
      street: 'Bahnhofplatz 3',
      zip: '4020',
      city: 'Linz',
      country: 'Austria',
      lat: 48.29101,
      long: 14.29021,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95'], MOCK_FUEL_TYPES['superPlus98']],
    openingHours: weekdays(1, '00:00:00', '23:59:59', [1, 2, 3, 4, 5, 6, 0]),
  },
  {
    id: 2,
    designation: 'OMV Linz Wiener Straße',
    address: {
      id: 2,
      street: 'Wiener Straße 311',
      zip: '4030',
      city: 'Linz',
      country: 'Austria',
      lat: 48.26354,
      long: 14.31528,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95'], MOCK_FUEL_TYPES['superPlus98'], MOCK_FUEL_TYPES['cng']],
    openingHours: weekdays(10, '06:00:00', '22:00:00', [1, 2, 3, 4, 5, 6]),
  },
  {
    id: 3,
    designation: 'BP Urfahr Freistädter Straße',
    address: {
      id: 3,
      street: 'Freistädter Straße 30',
      zip: '4040',
      city: 'Linz',
      country: 'Austria',
      lat: 48.31664,
      long: 14.28938,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95']],
    openingHours: weekdays(20, '05:30:00', '21:30:00', [1, 2, 3, 4, 5, 6, 0]),
  },
  {
    id: 4,
    designation: 'Shell Leonding Kremstal Straße',
    address: {
      id: 4,
      street: 'Kremstal Straße 4',
      zip: '4060',
      city: 'Leonding',
      country: 'Austria',
      lat: 48.27926,
      long: 14.25244,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95'], MOCK_FUEL_TYPES['superPlus98']],
    openingHours: weekdays(30, '06:00:00', '23:00:00', [1, 2, 3, 4, 5]),
  },
  {
    id: 5,
    designation: 'Jet Pasching Plus City',
    address: {
      id: 5,
      street: 'Pluskaufstraße 7',
      zip: '4061',
      city: 'Pasching',
      country: 'Austria',
      lat: 48.25584,
      long: 14.23286,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95'], MOCK_FUEL_TYPES['cng']],
    openingHours: weekdays(40, '07:00:00', '20:00:00', [1, 2, 3, 4, 5, 6]),
  },
  {
    id: 6,
    designation: 'Eni Linz Industriezeile',
    address: {
      id: 6,
      street: 'Industriezeile 76',
      zip: '4020',
      city: 'Linz',
      country: 'Austria',
      lat: 48.30577,
      long: 14.32856,
    },
    fuelTypes: [MOCK_FUEL_TYPES['diesel'], MOCK_FUEL_TYPES['super95'], MOCK_FUEL_TYPES['superPlus98']],
    openingHours: weekdays(50, '00:00:00', '23:59:59', [1, 2, 3, 4, 5, 6, 0]),
  },
];

/** Simulated network latency in ms, keeps loading states observable. */
const MOCK_DELAY_MS = 150;

@Injectable()
export class MockGasStationDataSource extends GasStationDataSource {
  override getAll(): Observable<GasStationDto[]> {
    return of(MOCK_GAS_STATIONS).pipe(delay(MOCK_DELAY_MS));
  }

  override getById(id: number): Observable<GasStationDto> {
    const station = MOCK_GAS_STATIONS.find((s) => s.id === id);
    if (!station) {
      return new Observable((subscriber) =>
        subscriber.error(new Error(`Gas station ${id} not found`)),
      );
    }
    return of(station).pipe(delay(MOCK_DELAY_MS));
  }
}
