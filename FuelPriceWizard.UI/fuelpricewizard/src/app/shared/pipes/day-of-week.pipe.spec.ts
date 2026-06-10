import { DayOfWeekPipe } from './day-of-week.pipe';

describe('DayOfWeekPipe', () => {
  const pipe = new DayOfWeekPipe();

  it('should map .NET DayOfWeek numbers (Sunday-first) to names', () => {
    expect(pipe.transform(0)).toBe('Sunday');
    expect(pipe.transform(1)).toBe('Monday');
    expect(pipe.transform(6)).toBe('Saturday');
  });

  it('should return an empty string for out-of-range values', () => {
    expect(pipe.transform(7)).toBe('');
    expect(pipe.transform(-1)).toBe('');
  });
});
