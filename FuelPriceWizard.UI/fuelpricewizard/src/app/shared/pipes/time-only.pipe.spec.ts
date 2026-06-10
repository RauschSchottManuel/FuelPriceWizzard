import { TimeOnlyPipe } from './time-only.pipe';

describe('TimeOnlyPipe', () => {
  const pipe = new TimeOnlyPipe();

  it('should shorten a TimeOnly string to hours and minutes', () => {
    expect(pipe.transform('06:30:00')).toBe('06:30');
    expect(pipe.transform('23:59:59')).toBe('23:59');
  });

  it('should pass through values without seconds', () => {
    expect(pipe.transform('06:30')).toBe('06:30');
  });

  it('should return an empty string for empty input', () => {
    expect(pipe.transform('')).toBe('');
    expect(pipe.transform(null)).toBe('');
    expect(pipe.transform(undefined)).toBe('');
  });
});
