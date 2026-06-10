import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Router, provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { GasStationDto } from '../../core/models/gas-station.model';
import { GasStationDataSource } from '../../core/services/gas-station.data-source';
import { MOCK_GAS_STATIONS } from '../../core/services/mock-gas-station.data-source';
import { OsmMapMarker } from '../../shared/osm-map/osm-map.component';
import { StationsPageComponent } from './stations-page.component';

describe('StationsPageComponent', () => {
  async function setup(stations: GasStationDto[]): Promise<ComponentFixture<StationsPageComponent>> {
    await TestBed.configureTestingModule({
      imports: [StationsPageComponent],
      providers: [
        provideRouter([]),
        { provide: GasStationDataSource, useValue: { getAll: () => of(stations) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(StationsPageComponent);
    fixture.detectChanges();
    return fixture;
  }

  function markers(fixture: ComponentFixture<StationsPageComponent>): OsmMapMarker[] {
    return fixture.componentInstance['markers']();
  }

  it('should load stations and render the list', async () => {
    const fixture = await setup(MOCK_GAS_STATIONS);

    expect(fixture.nativeElement.textContent).toContain(MOCK_GAS_STATIONS[0].designation);
    expect(fixture.debugElement.queryAll(By.css('app-station-list li')).length).toBe(
      MOCK_GAS_STATIONS.length,
    );
  });

  it('should compute one map marker per station with coordinates', async () => {
    const fixture = await setup(MOCK_GAS_STATIONS);

    const result = markers(fixture);
    expect(result.length).toBe(MOCK_GAS_STATIONS.length);
    expect(result[0].id).toBe(MOCK_GAS_STATIONS[0].id);
    expect(result[0].content).toContain(MOCK_GAS_STATIONS[0].designation);
  });

  it('should omit stations without coordinates from the map but keep them listed', async () => {
    const noCoords: GasStationDto = {
      ...MOCK_GAS_STATIONS[0],
      id: 99,
      designation: 'No Coords Station',
      address: { ...MOCK_GAS_STATIONS[0].address, lat: null, long: null },
    };
    const fixture = await setup([...MOCK_GAS_STATIONS, noCoords]);

    expect(markers(fixture).length).toBe(MOCK_GAS_STATIONS.length);
    expect(fixture.nativeElement.textContent).toContain('No Coords Station');
  });

  it('should escape HTML in marker popup content', async () => {
    const sneaky: GasStationDto = {
      ...MOCK_GAS_STATIONS[0],
      id: 98,
      designation: '<img src=x onerror=alert(1)>',
    };
    const fixture = await setup([sneaky]);

    expect(markers(fixture)[0].content).not.toContain('<img');
    expect(markers(fixture)[0].content).toContain('&lt;img');
  });

  it('should navigate to the detail page when a station is selected from the list', async () => {
    const fixture = await setup(MOCK_GAS_STATIONS);
    const router = TestBed.inject(Router);
    const navigateSpy = spyOn(router, 'navigate');

    fixture.debugElement.queryAll(By.css('app-station-list li button'))[1].nativeElement.click();

    expect(navigateSpy).toHaveBeenCalledWith(['/stations', MOCK_GAS_STATIONS[1].id]);
  });
});
