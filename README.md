[![](https://img.shields.io/nuget/v/soenneker.extensions.datetime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetime/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetime/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetime.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetime/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetime/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetime/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTime

A collection of helpful DateTime extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTime
```

## Quick start

```csharp
using Soenneker.Extensions.DateTime;

DateTime utcTime = DateTime.UtcNow;
var result = utcTime.ToTz(tzInfo);
```

## Common operations

- `ToTz()` - Converts a UTC `DateTime` to the specified time zone and then adjusts the result to UTC. This method is intended for scenarios where a UTC time needs to be converted to a specific time zone and then treated as if the converted time is in UTC.
- `ToUtc()` - Converts a `System.DateTime` value to Coordinated Universal Time (UTC) from a specified time zone, treating the original DateTime's kind as Unspecified. This method is useful when you have a DateTime value with a specific time zone and need to convert it to UTC, but the source DateTime's kind property is not Unspecified.
- `ToAge()` - Calculates the age in hours between the specified date and the current date and time. Returns the age in hours.
- `QuartersBetween()` - Returns the non-negative number of calendar quarters between two values, including fractional progress through the next quarter. Argument order does not matter.
- `YearsBetween()` - Returns the non-negative number of calendar years between two values, including fractional progress through the next year. Argument order does not matter.
- `MonthsBetween()` - Returns the non-negative number of calendar months between two values, including fractional progress through the next month. Argument order does not matter.
- `WholeMonthsBetween()` - Returns only fully completed calendar months; partial months are discarded and argument order does not matter.
- `WholeYearsBetween()` - Returns only fully completed calendar years; partial years are discarded and argument order does not matter.
- `WholeQuartersBetween()` - Returns only fully completed three-month quarters; partial quarters are discarded and argument order does not matter.
- `Trim()` - Trims a `System.DateTime` object to a specified level of precision. Returns a new `System.DateTime` object trimmed to the specified `unitOfTime`.
- `TrimEnd()` - Adjusts the provided `System.DateTime` object to the end of the specified period, minus one tick. Returns a new `System.DateTime` object representing the last moment of the specified period, just before it transitions to the next period, according to the specified `unitOfTime`.
- `Add()` - Adds a specified amount of time to the given `System.DateTime` object based on the provided `UnitOfTime`. Returns a new `System.DateTime` object that is the result of adding the specified amount of time to the original date and time.

The package also includes 28 additional operations for more specialized cases.
