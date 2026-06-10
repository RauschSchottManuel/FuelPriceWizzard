import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { GasStationDataSource } from './gas-station.data-source';
import { MOCK_GAS_STATIONS } from './mock-gas-station.data-source';
import { StationsStore } from './stations.store';

describe('StationsStore', () => {
  let getAllSpy: jasmine.Spy;

  function setup(dataSource: Partial<GasStationDataSource>): StationsStore {
    TestBed.configureTestingModule({
      providers: [{ provide: GasStationDataSource, useValue: dataSource }],
    });
    return TestBed.inject(StationsStore);
  }

  beforeEach(() => {
    getAllSpy = jasmine.createSpy('getAll').and.returnValue(of(MOCK_GAS_STATIONS));
  });

  it('should populate the stations signal on load', () => {
    const store = setup({ getAll: getAllSpy });

    store.load();

    expect(store.stations().length).toBe(MOCK_GAS_STATIONS.length);
    expect(store.loading()).toBeFalse();
    expect(store.error()).toBeUndefined();
  });

  it('should not re-fetch on a second load', () => {
    const store = setup({ getAll: getAllSpy });

    store.load();
    store.load();

    expect(getAllSpy).toHaveBeenCalledTimes(1);
  });

  it('should re-fetch when forced', () => {
    const store = setup({ getAll: getAllSpy });

    store.load();
    store.load(true);

    expect(getAllSpy).toHaveBeenCalledTimes(2);
  });

  it('should expose an error and allow retry after a failed load', () => {
    getAllSpy.and.returnValue(throwError(() => new Error('boom')));
    const store = setup({ getAll: getAllSpy });

    store.load();
    expect(store.error()).toBeDefined();
    expect(store.stations().length).toBe(0);

    getAllSpy.and.returnValue(of(MOCK_GAS_STATIONS));
    store.load();

    expect(store.error()).toBeUndefined();
    expect(store.stations().length).toBe(MOCK_GAS_STATIONS.length);
  });

  it('should resolve a station by id from the cache', () => {
    const store = setup({ getAll: getAllSpy });

    store.load();

    expect(store.stationById(2)()?.designation).toBe(MOCK_GAS_STATIONS[1].designation);
    expect(store.stationById(999)()).toBeUndefined();
  });
});
