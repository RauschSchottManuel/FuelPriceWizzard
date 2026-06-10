import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideCharts, withDefaultRegisterables } from 'ng2-charts';
import { of } from 'rxjs';
import { GasStationDataSource } from '../../../core/services/gas-station.data-source';
import { MOCK_GAS_STATIONS } from '../../../core/services/mock-gas-station.data-source';
import { PriceDataSource } from '../../../core/services/price.data-source';
import { latestPriceReading } from '../../../core/services/mock-price.generator';
import { StationDetailComponent } from './station-detail.component';

describe('StationDetailComponent', () => {
  async function setup(id: string): Promise<ComponentFixture<StationDetailComponent>> {
    await TestBed.configureTestingModule({
      imports: [StationDetailComponent],
      providers: [
        provideRouter([]),
        provideCharts(withDefaultRegisterables()),
        { provide: GasStationDataSource, useValue: { getAll: () => of(MOCK_GAS_STATIONS) } },
        {
          provide: PriceDataSource,
          useValue: {
            getLatestPrices: (stationId: number) =>
              of(
                MOCK_GAS_STATIONS.find((s) => s.id === stationId)?.fuelTypes.map((ft) =>
                  latestPriceReading(stationId, ft.id),
                ) ?? [],
              ),
            getPriceHistory: () => of([]),
          },
        },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(StationDetailComponent);
    fixture.componentRef.setInput('id', id);
    fixture.detectChanges();
    fixture.detectChanges();
    return fixture;
  }

  it('should render the station resolved from the route id', async () => {
    const fixture = await setup('2');
    const text = fixture.nativeElement.textContent;

    const station = MOCK_GAS_STATIONS[1];
    expect(text).toContain(station.designation);
    expect(text).toContain(station.address.street);
    expect(text).toContain(station.fuelTypes[0].displayValue);
  });

  it('should render opening hours and prices sections', async () => {
    const fixture = await setup('1');
    const text = fixture.nativeElement.textContent;

    expect(text).toContain('Opening hours');
    expect(text).toContain('Current prices');
    expect(text).toContain('Price history');
  });

  it('should show a not-found message for an unknown id', async () => {
    const fixture = await setup('999');

    expect(fixture.nativeElement.textContent).toContain('Station not found');
  });
});
