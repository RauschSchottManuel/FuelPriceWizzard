import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpGasStationDataSource } from './http-gas-station.data-source';
import { GasStationDto } from '../models/gas-station.model';
import { MOCK_GAS_STATIONS } from './mock-gas-station.data-source';

describe('HttpGasStationDataSource', () => {
  let dataSource: HttpGasStationDataSource;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), HttpGasStationDataSource],
    });
    dataSource = TestBed.inject(HttpGasStationDataSource);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should request all stations from /api/GasStations/all', () => {
    let result: GasStationDto[] | undefined;
    dataSource.getAll().subscribe((stations) => (result = stations));

    const req = httpMock.expectOne('/api/GasStations/all');
    expect(req.request.method).toBe('GET');
    req.flush(MOCK_GAS_STATIONS);

    expect(result?.length).toBe(MOCK_GAS_STATIONS.length);
    expect(result?.[0].designation).toBe(MOCK_GAS_STATIONS[0].designation);
  });

  it('should request a single station from /api/GasStations/{id}', () => {
    let result: GasStationDto | undefined;
    dataSource.getById(3).subscribe((station) => (result = station));

    const req = httpMock.expectOne('/api/GasStations/3');
    expect(req.request.method).toBe('GET');
    req.flush(MOCK_GAS_STATIONS[2]);

    expect(result?.id).toBe(3);
  });
});
