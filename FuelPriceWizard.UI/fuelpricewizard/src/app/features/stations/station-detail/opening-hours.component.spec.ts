import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { OpeningHoursDto } from '../../../core/models/gas-station.model';
import { OpeningHoursComponent } from './opening-hours.component';

describe('OpeningHoursComponent', () => {
  let fixture: ComponentFixture<OpeningHoursComponent>;

  const hours: OpeningHoursDto[] = [
    { id: 1, day: 1, from: '06:00:00', to: '22:00:00' },
    { id: 2, day: 6, from: '08:00:00', to: '12:00:00' },
    { id: 3, day: 6, from: '14:00:00', to: '18:00:00' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OpeningHoursComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(OpeningHoursComponent);
    fixture.componentRef.setInput('hours', hours);
    fixture.detectChanges();
  });

  it('should render the week Monday-first', () => {
    const dayCells = fixture.debugElement.queryAll(By.css('td:first-child'));
    expect(dayCells.length).toBe(7);
    expect(dayCells[0].nativeElement.textContent.trim()).toBe('Monday');
    expect(dayCells[6].nativeElement.textContent.trim()).toBe('Sunday');
  });

  it('should render open ranges without seconds and join multiple ranges', () => {
    const rows = fixture.debugElement.queryAll(By.css('tr'));
    const monday = rows[0].nativeElement.textContent;
    expect(monday).toContain('06:00–22:00');

    const saturday = rows[5].nativeElement.textContent;
    expect(saturday).toContain('08:00–12:00');
    expect(saturday).toContain('14:00–18:00');
  });

  it('should mark days without entries as closed', () => {
    const tuesday = fixture.debugElement.queryAll(By.css('tr'))[1].nativeElement.textContent;
    expect(tuesday).toContain('Closed');
  });
});
