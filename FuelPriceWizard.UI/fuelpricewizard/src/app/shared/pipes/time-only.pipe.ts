import { Pipe, PipeTransform } from '@angular/core';

/** Shortens a .NET TimeOnly string ("06:30:00") to "06:30". */
@Pipe({
  name: 'timeOnly',
  standalone: true,
})
export class TimeOnlyPipe implements PipeTransform {
  transform(time: string | null | undefined): string {
    if (!time) {
      return '';
    }
    const [hours, minutes] = time.split(':');
    return hours !== undefined && minutes !== undefined ? `${hours}:${minutes}` : time;
  }
}
