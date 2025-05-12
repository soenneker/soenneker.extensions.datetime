using System;
using System.Diagnostics.Contracts;
using Soenneker.Enums.UnitOfTime;

namespace Soenneker.Extensions.DateTime;

/// <summary>
/// A collection of helpful DateTime extension methods
/// </summary>
public static class DateTimeExtension
{
    private const double _ticksPerNanosecond = 100.0;
    private const double _ticksPerMicrosecond = 10.0;

    /// <summary>
    /// Converts a UTC <see cref="DateTime"/> to the specified time zone and then adjusts the result to UTC.
    /// </summary>
    /// <param name="utcTime">The UTC date and time to convert. Ensure this is correctly specified as UTC to avoid unintended results.</param>
    /// <param name="tzInfo">The target <see cref="TimeZoneInfo"/> representing the time zone to convert <paramref name="utcTime"/> into.</param>
    /// <returns>A <see cref="System.DateTime"/> instance representing the converted time, with the <see cref="System.DateTime.Kind"/> property set to <see cref="DateTimeKind.Utc"/>. 
    /// The time represented is the equivalent time in the specified time zone, but marked as UTC.</returns>
    /// <remarks>
    /// This method is intended for scenarios where a UTC time needs to be converted to a specific time zone and then treated as if the converted time is in UTC.
    /// This could be useful for displaying times in a uniform format while accounting for different time zones.
    /// Note that the returned <see cref="DateTime"/> does not represent the original UTC time but its equivalent in the specified time zone, marked as UTC.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown if <paramref name="tzInfo"/> is null.</exception>
    [Pure]
    public static System.DateTime ToTz(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        System.DateTime converted = System.TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzInfo);

        return converted.ToUtcKind();
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> value to Coordinated Universal Time (UTC) from a specified time zone, treating the original DateTime's kind as Unspecified.
    /// </summary>
    /// <param name="tzTime">The <see cref="System.DateTime"/> to convert to UTC. If <see cref="DateTimeKind"/> is not Unspecified, it will be forcibly changed to Unspecified before conversion, ignoring the original time zone indication.</param>
    /// <param name="tzInfo">The <see cref="System.TimeZoneInfo"/> representing the time zone of <paramref name="tzTime"/>. This is used to correctly calculate the UTC time, assuming <paramref name="tzTime"/> is in this specified time zone.</param>
    /// <returns>A <see cref="System.DateTime"/> value that represents the same point in time as <paramref name="tzTime"/>, expressed in UTC. The <see cref="DateTimeKind"/> of the returned DateTime is always Utc.</returns>
    /// <remarks>
    /// This method is useful when you have a DateTime value with a specific time zone and need to convert it to UTC, but the source DateTime's kind property is not Unspecified. The method first forces the kind to Unspecified to avoid double conversions by the .NET runtime, ensuring the conversion uses the specified <paramref name="tzInfo"/> accurately.
    /// Note: If the original <paramref name="tzTime"/> kind is Unspecified, it's directly used for conversion. It's recommended to explicitly manage DateTime kinds to prevent unintended behavior.
    /// <example>
    /// <code>
    /// var localTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Local);
    /// var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
    /// var utcTime = localTime.ToUtc(timeZoneInfo);
    /// Console.WriteLine(utcTime.Kind); // Outputs "Utc"
    /// Console.WriteLine(utcTime); // Outputs the UTC equivalent of the Eastern Standard Time provided date and time
    /// </code>
    /// </example>
    /// </remarks>
    [Pure]
    public static System.DateTime ToUtc(this System.DateTime tzTime, System.TimeZoneInfo tzInfo)
    {
        tzTime = tzTime.ToUnspecifiedKind();

        return System.TimeZoneInfo.ConvertTimeToUtc(tzTime, tzInfo);
    }

    /// <summary>
    /// Calculates the age in hours between the specified date and the current date and time.
    /// </summary>
    /// <param name="fromDateTime">The specified date and time.</param>
    /// <param name="unitOfTime"></param>
    /// <param name="utcNow">The current date and time in UTC. If not provided, the current UTC date and time will be used.</param>
    /// <returns>The age in hours.</returns>
    /// <exception cref="NotSupportedException"></exception>
    [Pure]
    public static double ToAge(this System.DateTime fromDateTime, UnitOfTime unitOfTime, System.DateTime? utcNow = null)
    {
        utcNow ??= System.DateTime.UtcNow;
        TimeSpan timeSpan = (utcNow - fromDateTime).Value;

        return unitOfTime.Name switch
        {
            nameof(UnitOfTime.Tick) => timeSpan.Ticks,
            nameof(UnitOfTime.Nanosecond) => timeSpan.Ticks * _ticksPerNanosecond,
            nameof(UnitOfTime.Microsecond) => timeSpan.Ticks / _ticksPerMicrosecond,
            nameof(UnitOfTime.Millisecond) => timeSpan.TotalMilliseconds,
            nameof(UnitOfTime.Second) => timeSpan.TotalSeconds,
            nameof(UnitOfTime.Minute) => timeSpan.TotalMinutes,
            nameof(UnitOfTime.Hour) => timeSpan.TotalHours,
            nameof(UnitOfTime.Day) => timeSpan.TotalDays,
            nameof(UnitOfTime.Week) => timeSpan.TotalHours / 7D,
            nameof(UnitOfTime.Month) => timeSpan.TotalDays / 30.44,
            nameof(UnitOfTime.Quarter) => timeSpan.TotalDays / (365.25 / 4D),
            nameof(UnitOfTime.Year) => timeSpan.TotalDays / 365.25,
            _ => throw new NotSupportedException("UnitOfTime is not supported for this method")
        };
    }

    /// <summary>
    /// Trims a <see cref="System.DateTime"/> object to a specified level of precision.
    /// </summary>
    /// <remarks>
    /// This method adjusts a <see cref="System.DateTime"/> object to the nearest lower value of the specified precision. For example, trimming to <see cref="UnitOfTime.Minute"/> 
    /// will result in a <see cref="System.DateTime"/> object set to the beginning of the minute, with seconds and milliseconds set to zero.
    /// The method supports various levels of precision, such as Year, Month, Day, Hour, Minute, and Second. Any time components finer than the specified precision are set to zero.
    /// The resulting <see cref="System.DateTime"/> is always returned with its <see cref="System.DateTime.Kind"/> property set to <see cref="DateTimeKind.Utc"/>.
    /// </remarks>
    /// <param name="dateTime">The <see cref="System.DateTime"/> to trim.</param>
    /// <param name="unitOfTime">The precision to which the <paramref name="dateTime"/> should be trimmed. This should be one of the values defined in <see cref="UnitOfTime"/>.</param>
    /// <param name="dateTimeKind"></param>
    /// <returns>A new <see cref="System.DateTime"/> object trimmed to the specified <paramref name="unitOfTime"/>.</returns>
    [Pure]
    public static System.DateTime Trim(this System.DateTime dateTime, UnitOfTime unitOfTime, DateTimeKind? dateTimeKind = null)
    {
        System.DateTime trimmed;

        dateTimeKind ??= dateTime.Kind;

        switch (unitOfTime.Name)
        {
            case nameof(UnitOfTime.Microsecond):
                {
                    long truncatedTicks = dateTime.Ticks - dateTime.Ticks % (long)_ticksPerMicrosecond;
                    trimmed = new System.DateTime(truncatedTicks, dateTime.Kind);
                    break;
                }
            case nameof(UnitOfTime.Millisecond):
                {
                    long truncatedTicks = dateTime.Ticks - dateTime.Ticks % 10000;
                    trimmed = new System.DateTime(truncatedTicks, dateTime.Kind);
                    break;
                }
            case nameof(UnitOfTime.Second):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Minute):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Hour):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Day):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Week): // Considering Monday is the first day - ISO 8601
            {
                int daysToSubtract = (int)dateTime.DayOfWeek - (int)DayOfWeek.Monday;
                if (daysToSubtract < 0)
                {
                    daysToSubtract += 7;
                }

                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTimeKind.Value).AddDays(-daysToSubtract);
                break;
            }
            case nameof(UnitOfTime.Month):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Quarter):
                // Determine the start month of the quarter
                int quarterNumber = (dateTime.Month - 1) / 3;
                int startMonthOfQuarter = quarterNumber * 3 + 1;
                trimmed = new System.DateTime(dateTime.Year, startMonthOfQuarter, 1, 0, 0, 0, 0, dateTime.Kind);
                break;
            case nameof(UnitOfTime.Year):
                trimmed = new System.DateTime(dateTime.Year, 1, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(UnitOfTime.Decade):
                // Calculate the start year of the decade
                int startYearOfDecade = dateTime.Year - dateTime.Year % 10;
                trimmed = new System.DateTime(startYearOfDecade, 1, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(unitOfTime), $"Unsupported UnitOfTime: {unitOfTime.Name}");
        }

        return trimmed;
    }

    /// <summary>
    /// Adjusts the provided <see cref="System.DateTime"/> object to the end of the specified period, minus one tick.
    /// </summary>
    /// <param name="dateTime">The date and time value to adjust.</param>
    /// <param name="unitOfTime">The precision level to which the date and time should be adjusted. This determines the period (e.g., Year, Month, Day, etc.) to which the <paramref name="dateTime"/> will be trimmed.</param>
    /// <param name="dateTimeKind">Optional. Specifies the kind of date and time adjustment. If not provided, the kind of <paramref name="dateTime"/> will be used. This can influence the handling of time zones.</param>
    /// <returns>
    /// A new <see cref="System.DateTime"/> object representing the last moment of the specified period, just before it transitions to the next period, according to the specified <paramref name="unitOfTime"/>.
    /// </returns>
    /// <remarks>
    /// This method first calculates the start of the next period based on the specified <paramref name="unitOfTime"/> and <paramref name="dateTimeKind"/>, if provided. It then subtracts one tick from this calculated start time to get the precise end of the current period.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <paramref name="unitOfTime"/> is provided.</exception>
    [Pure]
    public static System.DateTime TrimEnd(this System.DateTime dateTime, UnitOfTime unitOfTime, DateTimeKind? dateTimeKind = null)
    {
        System.DateTime startOfPeriod = dateTime.Trim(unitOfTime, dateTimeKind);

        startOfPeriod = unitOfTime.Name switch
        {
            nameof(UnitOfTime.Microsecond) => startOfPeriod.AddTicks((long)_ticksPerMicrosecond), // Add 10 ticks to move to the start of the next microsecond
            nameof(UnitOfTime.Millisecond) => startOfPeriod.AddMilliseconds(1),
            nameof(UnitOfTime.Second) => startOfPeriod.AddSeconds(1),
            nameof(UnitOfTime.Minute) => startOfPeriod.AddMinutes(1),
            nameof(UnitOfTime.Hour) => startOfPeriod.AddHours(1),
            nameof(UnitOfTime.Day) => startOfPeriod.AddDays(1),
            nameof(UnitOfTime.Week) => startOfPeriod.AddDays(7),
            nameof(UnitOfTime.Month) => startOfPeriod.AddMonths(1),
            nameof(UnitOfTime.Quarter) => startOfPeriod.AddMonths(3), // Quarters consist of 3 months
            nameof(UnitOfTime.Year) => startOfPeriod.AddYears(1),
            nameof(UnitOfTime.Decade) => startOfPeriod.AddYears(10),
            _ => throw new ArgumentOutOfRangeException(nameof(unitOfTime), $"Unsupported UnitOfTime: {unitOfTime.Name}")
        };

        // Subtract one tick to get the last moment of the current period
        return startOfPeriod.AddTicks(-1);
    }

    /// <summary>
    /// Adds a specified amount of time to the given <see cref="System.DateTime"/> object based on the provided <see cref="UnitOfTime"/>.
    /// </summary>
    /// <param name="dateTime">The original <see cref="System.DateTime"/> object to which time will be added.</param>
    /// <param name="value">The amount of time to add. Can be a fractional value for finer granularity.</param>
    /// <param name="unitOfTime">The unit of time to add, specified as a <see cref="UnitOfTime"/>.</param>
    /// <returns>A new <see cref="System.DateTime"/> object that is the result of adding the specified amount of time to the original date and time.</returns>
    [Pure]
    public static System.DateTime Add(this System.DateTime dateTime, double value, UnitOfTime unitOfTime)
    {
        switch (unitOfTime.Name)
        {
            case nameof(UnitOfTime.Tick):
                return dateTime.AddTicks((long)value);
            case nameof(UnitOfTime.Nanosecond):
                double totalTicksForNanoseconds = value / _ticksPerNanosecond;
                var wholeTicksForNanoseconds = (long)totalTicksForNanoseconds;
                double fractionalTicksForNanoseconds = totalTicksForNanoseconds - wholeTicksForNanoseconds;
                dateTime = dateTime.AddTicks(wholeTicksForNanoseconds);
                return dateTime.AddTicks((long)(fractionalTicksForNanoseconds * _ticksPerNanosecond));
            case nameof(UnitOfTime.Microsecond):
                double totalTicksForMicroseconds = value * _ticksPerMicrosecond;
                var wholeTicksForMicroseconds = (long)totalTicksForMicroseconds;
                double fractionalTicksForMicroseconds = totalTicksForMicroseconds - wholeTicksForMicroseconds;
                dateTime = dateTime.AddTicks(wholeTicksForMicroseconds);
                return dateTime.AddTicks((long)(fractionalTicksForMicroseconds * _ticksPerMicrosecond));
            case nameof(UnitOfTime.Millisecond):
                return dateTime.AddMilliseconds(value);
            case nameof(UnitOfTime.Second):
                return dateTime.AddSeconds(value);
            case nameof(UnitOfTime.Minute):
                return dateTime.AddMinutes(value);
            case nameof(UnitOfTime.Hour):
                return dateTime.AddHours(value);
            case nameof(UnitOfTime.Day):
                return dateTime.AddDays(value);
            case nameof(UnitOfTime.Week):
                return dateTime.AddDays(value * 7);
            case nameof(UnitOfTime.Month):
                var wholeMonths = (int)value;
                double fractionalMonths = value - wholeMonths;
                dateTime = dateTime.AddMonths(wholeMonths);
                return dateTime.AddDays(fractionalMonths * System.DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
            case nameof(UnitOfTime.Quarter):
                return dateTime.AddMonths((int)(value * 3));
            case nameof(UnitOfTime.Year):
                var wholeYears = (int)value;
                double fractionalYears = value - wholeYears;
                dateTime = dateTime.AddYears(wholeYears);
                return dateTime.AddDays(fractionalYears * (System.DateTime.IsLeapYear(dateTime.Year) ? 366 : 365));
            case nameof(UnitOfTime.Decade):
                return dateTime.AddYears((int)(value * 10));
            default:
                throw new ArgumentOutOfRangeException(nameof(unitOfTime), $"Unsupported UnitOfTime: {unitOfTime.Name}");
        }
    }

    [Pure]
    public static System.DateTime Subtract(this System.DateTime dateTime, double value, UnitOfTime unitOfTime)
    {
        return dateTime.Add(-value, unitOfTime);
    }

    /// <inheritdoc cref="Trim(System.DateTime, UnitOfTime, DateTimeKind?)"/>
    [Pure]
    public static System.DateTime ToStartOf(this System.DateTime dateTime, UnitOfTime unitOfTime, DateTimeKind? dateTimeKind = null)
    {
        return Trim(dateTime, unitOfTime, dateTimeKind);
    }

    /// <inheritdoc cref="TrimEnd(System.DateTime, UnitOfTime, DateTimeKind?)"/>
    [Pure]
    public static System.DateTime ToEndOf(this System.DateTime dateTime, UnitOfTime unitOfTime, DateTimeKind? dateTimeKind = null)
    {
        return TrimEnd(dateTime, unitOfTime, dateTimeKind);
    }

    /// <summary>
    /// Essentially wraps <see cref="System.DateTime.SpecifyKind"/> in extension method
    /// </summary>
    [Pure]
    public static System.DateTime ToUtcKind(this System.DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Utc)
            return dateTime;

        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// Essentially wraps <see cref="System.DateTime.SpecifyKind"/> in extension method
    /// </summary>
    [Pure]
    public static System.DateTime ToUnspecifiedKind(this System.DateTime dateTime)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            return dateTime;

        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> object to a <see cref="System.DateTimeOffset"/> object.
    /// </summary>
    /// <remarks>
    /// This method creates a <see cref="System.DateTimeOffset"/> object from the provided <see cref="System.DateTime"/>.
    /// The <see cref="System.DateTime"/> object is assumed to be in the local time zone if it is unspecified or specified as local.
    /// If the <see cref="System.DateTime"/> object is specified as UTC, the resulting <see cref="DateTimeOffset"/>
    /// will have an offset of zero.
    /// </remarks>
    /// <param name="dateTime">The <see cref="System.DateTime"/> object to convert.</param>
    /// <returns>A <see cref="System.DateTimeOffset"/> object that represents the same point in time as the <paramref name="dateTime"/> parameter.</returns>
    [Pure]
    public static DateTimeOffset ToDateTimeOffset(this System.DateTime dateTime)
    {
        return new DateTimeOffset(dateTime);
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> object to the number of seconds that have elapsed since the Unix epoch (1970-01-01T00:00:00Z).
    /// </summary>
    /// <remarks>
    /// This method first converts the <see cref="System.DateTime"/> object to a <see cref="DateTimeOffset"/> to accurately account for time zone differences
    /// before calculating the Unix time. The input <see cref="System.DateTime"/> should be in UTC to ensure an accurate conversion to Unix time seconds.
    /// </remarks>
    /// <param name="utc">The UTC <see cref="System.DateTime"/> to convert to Unix time seconds.</param>
    /// <returns>The number of seconds that have elapsed since 1970-01-01T00:00:00Z, represented as a long.</returns>
    [Pure]
    public static long ToUnixTimeSeconds(this System.DateTime utc)
    {
        return utc.ToDateTimeOffset().ToUnixTimeSeconds();
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.DateTime"/> value falls within a given date range, inclusive of the start and end dates.
    /// </summary>
    /// <param name="dateTime">The <see cref="System.DateTime"/> instance to check.</param>
    /// <param name="startAt">The start date of the range. The check is inclusive, meaning this date is considered part of the range.</param>
    /// <param name="endAt">The end date of the range. The check is inclusive, meaning this date is considered part of the range.</param>
    /// <returns><c>true</c> if <paramref name="dateTime"/> is equal to or falls between <paramref name="startAt"/> and <paramref name="endAt"/>; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// This method extends <see cref="System.DateTime"/> and can be used to easily determine if a date occurs within a specified range, including the boundaries.
    /// <example>
    /// <code>
    /// var dateToCheck = new DateTime(2023, 3, 15);
    /// var startDate = new DateTime(2023, 3, 1);
    /// var endDate = new DateTime(2023, 3, 31);
    /// bool isInMarch = dateToCheck.IsBetween(startDate, endDate);
    /// Console.WriteLine(isInMarch); // Outputs "True"
    /// </code>
    /// </example>
    /// </remarks>
    [Pure]
    public static bool IsBetween(this System.DateTime dateTime, System.DateTime startAt, System.DateTime endAt)
    {
        return dateTime >= startAt && dateTime <= endAt;
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> instance to an integer in the format yyyyMMdd.
    /// </summary>
    /// <param name="dateTime">The <see cref="System.DateTime"/> instance to convert.</param>
    /// <returns>An integer representing the <paramref name="dateTime"/> in the format yyyyMMdd.</returns>
    /// <exception cref="FormatException">Thrown when the conversion to the integer format fails, which should not occur with valid dates.</exception>
    /// <remarks>
    /// This method extends <see cref="System.DateTime"/> and allows for a compact representation of a date as an integer. This can be useful for comparisons, sorting, or storing dates in a condensed format.
    /// <example>
    /// <code>
    /// var date = new DateTime(2023, 3, 15);
    /// int dateInt = date.ToDateInteger();
    /// Console.WriteLine(dateInt); // Outputs "20230315"
    /// </code>
    /// </example>
    /// </remarks>
    [Pure]
    public static int ToDateAsInteger(this System.DateTime dateTime)
    {
        var str = dateTime.ToString("yyyyMMdd");
        return int.Parse(str);
    }

    /// <summary>
    /// Calculates the whole hour part of the time zone offset for a given UTC date and time and a specified time zone.
    /// </summary>
    /// <remarks>
    /// This method provides the time zone offset in hours for the specified UTC date and time, accounting for any applicable daylight saving time changes.
    /// The offset is determined by first converting the UTC time to the target time zone and then calculating the offset from UTC.
    /// It is designed to be timezone-aware, adjusting the result based on the time zone's rules for daylight saving time.
    /// </remarks>
    /// <param name="utcNow">The UTC date and time to calculate the offset for. If null, the current UTC time is used.</param>
    /// <param name="timeZoneInfo">The time zone to calculate the offset against.</param>
    /// <returns>The time zone offset in hours from UTC. Time zones west of UTC return negative values. i.e. Eastern returns a negative value (-4, or -5)</returns>
    [Pure]
    public static int ToTzOffsetHours(this System.DateTime utcNow, System.TimeZoneInfo timeZoneInfo)
    {
        return utcNow.ToTzOffset(timeZoneInfo).Hours;
    }

    /// <summary>
    /// Determines the time zone offset as a TimeSpan for a given UTC date and time in the specified time zone, considering daylight saving time.
    /// </summary>
    /// <remarks>
    /// This method calculates the exact time zone offset, including minutes and seconds, for the specified UTC date and time.
    /// It accounts for the time zone's daylight saving rules, which can cause the offset to vary throughout the year.
    /// The method uses the provided <paramref name="timeZoneInfo"/> to convert the <paramref name="utcNow"/> to local time and then calculates the offset from UTC.
    /// If <paramref name="utcNow"/> is null, the current UTC time is used for the calculation.
    /// This is useful for applications needing precise time zone offset information, including minute adjustments for DST.
    /// </remarks>
    /// <param name="utcNow">The UTC date and time to calculate the offset for, or null to use the current UTC time.</param>
    /// <param name="timeZoneInfo">The time zone to calculate the offset for.</param>
    [Pure]
    public static TimeSpan ToTzOffset(this System.DateTime utcNow, System.TimeZoneInfo timeZoneInfo)
    {
        return timeZoneInfo.GetUtcOffset(utcNow.ToTz(timeZoneInfo));
    }

    /// <summary>
    /// Converts a specific hour in a given time zone to its corresponding hour in UTC.
    /// </summary>
    /// <remarks>
    /// This method calculates the UTC equivalent of a specified hour in a given time zone, considering the time zone's offset from UTC, including any daylight saving time adjustments.
    /// It is designed to handle time zone differences and daylight saving time, ensuring that the conversion always produces a valid hour in the 24-hour format.
    /// Special Case: Due to the modulo operation, a conversion can result in 24, which represents midnight at the start of a new day.
    /// </remarks>
    /// <param name="utcNow">The current UTC date and time, used to determine the time zone's current offset, including daylight saving time.</param>
    /// <param name="tzHour">The hour in the specified time zone to be converted to UTC. Must be in 24-hour format.</param>
    /// <param name="timeZoneInfo">The time zone of the original hour.</param>
    /// <returns>The hour in UTC after conversion. This is always a positive number in the 24-hour format, where 24 may indicate midnight.</returns>
    [Pure]
    public static int ToUtcHoursFromTz(this System.DateTime utcNow, int tzHour, System.TimeZoneInfo timeZoneInfo)
    {
        int utcHoursOffset = utcNow.ToTzOffsetHours(timeZoneInfo);
        return (tzHour - utcHoursOffset) % 24;
    }

    /// <summary>
    /// Subtracts an amount (delay) of time (endAt), and then adds subtracts another amount (subtraction) of time (startAt).
    /// </summary>
    [Pure]
    public static (System.DateTime startAt, System.DateTime endAt) ToWindow(this System.DateTime dateTime, int delay, int subtraction, UnitOfTime unitOfTime)
    {
        System.DateTime endAt = dateTime.Subtract(delay, unitOfTime);
        System.DateTime startAt = endAt.Subtract(subtraction, unitOfTime);

        return (startAt, endAt);
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> to a <see cref="DateOnly"/> by stripping the time component.
    /// </summary>
    /// <param name="dateTime">The <see cref="System.DateTime"/> to convert.</param>
    /// <returns>A <see cref="DateOnly"/> representing the date portion of the input.</returns>
    [Pure]
    public static DateOnly ToDateOnly(this System.DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }
}