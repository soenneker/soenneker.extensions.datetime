[![](https://img.shields.io/nuget/v/soenneker.extensions.datetime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetime/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetime/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetime/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetime/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTime

Date-time helpers for time-zone wall clocks, calendar differences, period boundaries, unit-based arithmetic, offsets, and common string formats.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTime
```

## Time-zone conversion

```csharp
using Soenneker.Extensions.DateTime;

TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

System.DateTime utc = new(2026, 8, 29, 18, 0, 0, DateTimeKind.Utc);
System.DateTime easternWallClock = utc.ToTz(eastern);

System.DateTime enteredWallClock = new(2026, 8, 29, 14, 0, 0);
System.DateTime convertedUtc = enteredWallClock.ToUtc(eastern);
```

`ToUtc()` interprets the input fields as a wall-clock value in the supplied time zone. Its original `Kind` is ignored, and the result is a UTC instant.

`ToTz()` has intentionally different semantics from a normal instant-preserving conversion: it converts a UTC instant to the target wall clock and then labels that wall-clock value as `DateTimeKind.Utc`. The returned fields show target-zone time, but the value must not be persisted or compared as the original UTC instant. Use `DateTimeOffset` or `TimeZoneInfo.ConvertTimeFromUtc()` when you need conventional instant semantics.

## Differences and age

```csharp
System.DateTime createdAt = new(2025, 1, 15, 12, 0, 0, DateTimeKind.Utc);
System.DateTime measuredAt = new(2026, 3, 1, 12, 0, 0, DateTimeKind.Utc);

double ageInDays = createdAt.ToAge(UnitOfTime.Day, measuredAt);
double months = DateTimeExtension.MonthsBetween(createdAt, measuredAt);
int completeYears = DateTimeExtension.WholeYearsBetween(createdAt, measuredAt);
```

`ToAge()` supports ticks through years. Fixed units use elapsed duration; months, quarters, and years use calendar intervals. Past values are positive and future values are negative.

`MonthsBetween()`, `QuartersBetween()`, and `YearsBetween()` are non-negative regardless of argument order and include fractional progress through the next calendar interval. Their `Whole...Between()` counterparts discard the incomplete interval.

## Period boundaries

```csharp
System.DateTime value = new(2026, 8, 29, 16, 42, 30, DateTimeKind.Utc);

System.DateTime startOfMonth = value.ToStartOf(UnitOfTime.Month);
System.DateTime endOfMonth = value.ToEndOf(UnitOfTime.Month);
System.DateTime startOfWeek = value.Trim(UnitOfTime.Week);
```

`Trim()` and `ToStartOf()` return the first tick of the selected period. `TrimEnd()` and `ToEndOf()` return one tick before the next period. Weeks begin on Monday, quarters begin in January/April/July/October, and decades begin at years divisible by ten. The input `Kind` is preserved unless the optional `dateTimeKind` argument overrides it.

Boundary operations support microseconds, milliseconds, seconds, minutes, hours, days, weeks, months, quarters, years, and decades.

## Unit-based arithmetic

```csharp
System.DateTime delayed = value.Add(1.5, UnitOfTime.Hour);
System.DateTime previousQuarter = value.Subtract(1, UnitOfTime.Quarter);
(System.DateTime startAt, System.DateTime endAt) =
    value.ToWindow(delay: 5, subtraction: 30, UnitOfTime.Minute);
```

`Add()` and `Subtract()` support ticks, nanoseconds, microseconds, milliseconds, seconds, minutes, hours, days, weeks, months, quarters, years, and decades. `DateTime` stores 100-nanosecond ticks, so sub-tick portions of nanosecond or microsecond inputs are truncated. Fractional months and years are converted using the length of the calendar month or year reached after adding the whole portion.

`ToWindow()` first subtracts `delay` to produce `endAt`, then subtracts `subtraction` from that result to produce `startAt`.

## Other helpers

- `IsBetween()` uses inclusive start and end boundaries.
- `ToDateAsInteger()` returns `yyyyMMdd` as an integer.
- `ToUnixTimeSeconds()` uses `DateTimeOffset` conversion rules, so the input `Kind` matters.
- `ToDateTimeOffset()` follows the framework constructor: UTC receives offset zero; Local and Unspecified use the machine's local zone.
- `ToUtcKind()` and `ToUnspecifiedKind()` only relabel `Kind`; they do not convert clock fields.
- `ToTzOffset()` returns the applicable offset for the supplied UTC instant, including daylight-saving rules.
- `ToTzOffsetHours()` returns only the whole-hour component; use `ToTzOffset()` for half-hour and quarter-hour zones.
- `ToUtcHoursFromTz()` converts a local hour to a UTC hour from `0` through `23` using the offset applicable at the supplied instant.

## Formatting

The formatting helpers include:

| Method | Output |
| --- | --- |
| `ToPreciseFormat()` | `yyyy-MM-ddTHH:mm:ss.fffffff` |
| `ToPreciseUtcFormat()` | Same format plus literal `Z` |
| `ToIso8601()` / `ToWebString()` | `yyyy-MM-ddTHH:mm:ss.fffZ` |
| `ToMonthFirstDateFormat()` | `MM-dd-yyyy` |
| `ToFileName()` | `yyyy-MM-dd--HH-mm-ss` |
| `ToShortMonthDayYearString()` | Invariant `MMM dd, yyyy` |
| `ToLongMonthDayYearString()` | Invariant `MMMM d, yyyy` |

The UTC-named formatters append a literal `Z`; they do not convert the value first. The `ToTz...Format()` methods perform the library's `ToTz()` wall-clock conversion and append the time-zone abbreviation supplied by `Soenneker.Extensions.TimeZoneInfos`.
