import { ComponentRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { GasStationDto } from '../../core/models/gas-station.model';
import { MOCK_GAS_STATIONS } from '../../core/services/mock-gas-station.data-source';
import { StationListComponent } from './station-list.component';

describe('StationListComponent', () => {
  let fixture: ComponentFixture<StationListComponent>;
  let component: StationListComponent;
  let componentRef: ComponentRef<StationListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StationListComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(StationListComponent);
    component = fixture.componentInstance;
    componentRef = fixture.componentRef;
    componentRef.setInput('stations', MOCK_GAS_STATIONS);
    fixture.detectChanges();
  });

  function listedStationNames(): string[] {
    return fixture.debugElement
      .queryAll(By.css('li button > span:first-child'))
      .map((el) => (el.nativeElement.textContent as string).trim());
  }

  function search(term: string): void {
    const input: HTMLInputElement = fixture.debugElement.query(By.css('input')).nativeElement;
    input.value = term;
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
  }

  it('should render all stations', () => {
    expect(listedStationNames().length).toBe(MOCK_GAS_STATIONS.length);
  });

  it('should filter by designation', () => {
    search('omv');
    const names = listedStationNames();
    expect(names.length).toBe(1);
    expect(names[0]).toContain('OMV');
  });

  it('should filter by city', () => {
    search('leonding');
    const names = listedStationNames();
    expect(names.length).toBe(1);
    expect(names[0]).toContain('Shell');
  });

  it('should show an empty state when nothing matches', () => {
    search('does-not-exist');
    expect(listedStationNames().length).toBe(0);
    expect(fixture.nativeElement.textContent).toContain('No stations match');
  });

  it('should emit stationSelected on click', () => {
    const selected: GasStationDto[] = [];
    component.stationSelected.subscribe((station: GasStationDto) => selected.push(station));

    fixture.debugElement.queryAll(By.css('li button'))[1].nativeElement.click();

    expect(selected.length).toBe(1);
    expect(selected[0].id).toBe(MOCK_GAS_STATIONS[1].id);
  });
});
