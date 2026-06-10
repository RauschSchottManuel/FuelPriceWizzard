import { Pipe, PipeTransform } from '@angular/core';

const DAY_NAMES = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

/** Maps a .NET DayOfWeek number (0 = Sunday … 6 = Saturday) to its English name. */
@Pipe({
  name: 'dayOfWeek',
  standalone: true,
})
export class DayOfWeekPipe implements PipeTransform {
  transform(day: number): string {
    return DAY_NAMES[day] ?? '';
  }
}
