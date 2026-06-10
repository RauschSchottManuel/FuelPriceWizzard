import { fakeAsync, tick } from '@angular/core/testing';
import { GasStationDto } from '../models/gas-station.model';
import { MOCK_GAS_STATIONS, MockGasStationDataSource } from './mock-gas-station.data-source';

describe('MockGasStationDataSource', () => {
  let dataSource: MockGasStationDataSource;

  beforeEach(() => {
    dataSource = new MockGasStationDataSource();
  });

  it('should return all mock stations', fakeAsync(() => {
    let result: GasStationDto[] | undefined;
    dataSource.getAll().subscribe((stations) => (result = stations));
    tick(200);

    expect(result?.length).toBe(MOCK_GAS_STATIONS.length);
  }));

  it('should provide coordinates for every mock station', () => {
    for (const station of MOCK_GAS_STATIONS) {
      expect(station.address.lat).withContext(station.designation).toBeDefined();
      expect(station.address.long).withContext(station.designation).toBeDefined();
    }
  });

  it('should return a station by id', fakeAsync(() => {
    let result: GasStationDto | undefined;
    dataSource.getById(2).subscribe((station) => (result = station));
    tick(200);

    expect(result?.id).toBe(2);
  }));

  it('should error for an unknown id', fakeAsync(() => {
    let error: Error | undefined;
    dataSource.getById(999).subscribe({ error: (e) => (error = e) });
    tick(200);

    expect(error).toBeDefined();
  }));
});
