import { Component, computed, input } from '@angular/core';
import { OpeningHoursDto } from '../../../core/models/gas-station.model';
import { DayOfWeekPipe } from '../../../shared/pipes/day-of-week.pipe';
import { TimeOnlyPipe } from '../../../shared/pipes/time-only.pipe';

interface DayRow {
  day: number;
  entries: OpeningHoursDto[];
}

@Component({
  selector: 'app-opening-hours',
  standalone: true,
  imports: [DayOfWeekPipe, TimeOnlyPipe],
  templateUrl: './opening-hours.component.html',
})
export class OpeningHoursComponent {
  public hours = input.required<OpeningHoursDto[]>();

  /** Monday-first week; .NET DayOfWeek is Sunday-first (0 = Sunday). */
  protected readonly week = computed<DayRow[]>(() => {
    const mondayFirst = [1, 2, 3, 4, 5, 6, 0];
    const hours = this.hours();
    return mondayFirst.map((day) => ({
      day,
      entries: hours.filter((entry) => entry.day === day),
    }));
  });
}
