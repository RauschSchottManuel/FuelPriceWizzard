import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { HttpPriceDataSource } from './http-price.data-source';

describe('HttpPriceDataSource', () => {
  let dataSource: HttpPriceDataSource;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting(), HttpPriceDataSource],
    });
    dataSource = TestBed.inject(HttpPriceDataSource);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should request latest prices from /api/PriceReadings/latest/{stationId}', () => {
    dataSource.getLatestPrices(5).subscribe();

    const req = httpMock.expectOne('/api/PriceReadings/latest/5');
    expect(req.request.method).toBe('GET');
    req.flush([]);
  });

  it('should request history with from/to query parameters', () => {
    const from = new Date('2025-03-01T00:00:00Z');
    const to = new Date('2025-03-08T00:00:00Z');

    dataSource.getPriceHistory(5, 2, from, to).subscribe();

    const req = httpMock.expectOne(
      (r) => r.url === '/api/PriceReadings/history/5/2' && r.method === 'GET',
    );
    expect(req.request.params.get('from')).toBe(from.toISOString());
    expect(req.request.params.get('to')).toBe(to.toISOString());
    req.flush([]);
  });
});
