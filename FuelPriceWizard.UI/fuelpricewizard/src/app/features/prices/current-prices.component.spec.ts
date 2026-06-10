import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of } from 'rxjs';
import { MOCK_GAS_STATIONS } from '../../core/services/mock-gas-station.data-source';
import { latestPriceReading } from '../../core/services/mock-price.generator';
import { PriceDataSource } from '../../core/services/price.data-source';
import { CurrentPricesComponent } from './current-prices.component';

describe('CurrentPricesComponent', () => {
  let fixture: ComponentFixture<CurrentPricesComponent>;
  let getLatestPricesSpy: jasmine.Spy;

  const station = MOCK_GAS_STATIONS[1]; // four fuel types

  beforeEach(async () => {
    getLatestPricesSpy = jasmine
      .createSpy('getLatestPrices')
      .and.callFake((stationId: number) =>
        of(station.fuelTypes.map((ft) => latestPriceReading(stationId, ft.id))),
      );

    await TestBed.configureTestingModule({
      imports: [CurrentPricesComponent],
      providers: [{ provide: PriceDataSource, useValue: { getLatestPrices: getLatestPricesSpy } }],
    }).compileComponents();

    fixture = TestBed.createComponent(CurrentPricesComponent);
    fixture.componentRef.setInput('station', station);
    fixture.detectChanges();
    fixture.detectChanges();
  });

  it('should request the latest prices for the station', () => {
    expect(getLatestPricesSpy).toHaveBeenCalledWith(station.id);
  });

  it('should render one row per fuel type', () => {
    const rows = fixture.debugElement.queryAll(By.css('tbody tr'));
    expect(rows.length).toBe(station.fuelTypes.length);
  });

  it('should render fuel type names, 3-decimal prices, and the currency symbol', () => {
    const firstRow = fixture.debugElement.query(By.css('tbody tr')).nativeElement.textContent;
    expect(firstRow).toContain(station.fuelTypes[0].displayValue);
    expect(firstRow).toMatch(/\d+\.\d{3}\s*€/);
  });
});
