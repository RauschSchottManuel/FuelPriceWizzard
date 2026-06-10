import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { of } from 'rxjs';
import { MOCK_GAS_STATIONS } from '../../core/services/mock-gas-station.data-source';
import { generatePriceHistory } from '../../core/services/mock-price.generator';
import { PriceDataSource } from '../../core/services/price.data-source';
import { PriceHistoryChartComponent } from './price-history-chart.component';

describe('PriceHistoryChartComponent', () => {
  let fixture: ComponentFixture<PriceHistoryChartComponent>;
  let component: PriceHistoryChartComponent;
  let getPriceHistorySpy: jasmine.Spy;

  const station = MOCK_GAS_STATIONS[1]; // four fuel types

  beforeEach(async () => {
    getPriceHistorySpy = jasmine
      .createSpy('getPriceHistory')
      .and.callFake((stationId: number, fuelTypeId: number, from: Date, to: Date) =>
        of(generatePriceHistory(stationId, fuelTypeId, from, to)),
      );

    await TestBed.configureTestingModule({
      imports: [PriceHistoryChartComponent],
      providers: [
        provideCharts(withDefaultRegisterables()),
        { provide: PriceDataSource, useValue: { getPriceHistory: getPriceHistorySpy } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PriceHistoryChartComponent);
    component = fixture.componentInstance;
    fixture.componentRef.setInput('station', station);
    fixture.detectChanges();
    fixture.detectChanges();
  });

  function rangeButton(label: string): HTMLButtonElement {
    return fixture.debugElement
      .queryAll(By.css('div[aria-label="Time range"] button'))
      .find((el) => el.nativeElement.textContent.trim() === label)!.nativeElement;
  }

  function fuelTypeButtons(): HTMLButtonElement[] {
    return fixture.debugElement
      .queryAll(By.css('div[aria-label="Fuel type"] button'))
      .map((el) => el.nativeElement);
  }

  it('should default to the first fuel type and a 30-day range', () => {
    expect(getPriceHistorySpy).toHaveBeenCalledTimes(1);
    const [stationId, fuelTypeId, from, to] = getPriceHistorySpy.calls.mostRecent().args;
    expect(stationId).toBe(station.id);
    expect(fuelTypeId).toBe(station.fuelTypes[0].id);
    expect(Math.round((to.getTime() - from.getTime()) / (24 * 60 * 60 * 1000))).toBe(30);
  });

  it('should map readings to chart points', () => {
    const data = component['chartData']();
    const [, , from, to] = getPriceHistorySpy.calls.mostRecent().args;
    const expected = generatePriceHistory(station.id, station.fuelTypes[0].id, from, to);

    expect(data.datasets[0].data.length).toBe(expected.length);
    expect(data.datasets[0].label).toBe(station.fuelTypes[0].displayValue);
  });

  it('should render one button per fuel type of the station', () => {
    expect(fuelTypeButtons().length).toBe(station.fuelTypes.length);
  });

  it('should reload when the range changes', () => {
    rangeButton('7d').click();
    fixture.detectChanges();

    expect(getPriceHistorySpy).toHaveBeenCalledTimes(2);
    const [, , from, to] = getPriceHistorySpy.calls.mostRecent().args;
    expect(Math.round((to.getTime() - from.getTime()) / (24 * 60 * 60 * 1000))).toBe(7);
  });

  it('should show a message instead of the chart when the station has no fuel types', () => {
    fixture.componentRef.setInput('station', { ...station, fuelTypes: [] });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('No fuel types known');
    expect(fixture.debugElement.query(By.css('canvas'))).toBeNull();
    expect(fixture.nativeElement.textContent).not.toContain('Loading price history');
  });

  it('should reload when another fuel type is selected', () => {
    fuelTypeButtons()[2].click();
    fixture.detectChanges();

    expect(getPriceHistorySpy).toHaveBeenCalledTimes(2);
    const [, fuelTypeId] = getPriceHistorySpy.calls.mostRecent().args;
    expect(fuelTypeId).toBe(station.fuelTypes[2].id);
  });
});
