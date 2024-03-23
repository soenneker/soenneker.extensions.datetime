using System;
using System.Diagnostics.Contracts;
using Soenneker.Enums.DateTimePrecision;

namespace Soenneker.Extensions.DateTime;

/// <summary>
/// A collection of helpful DateTime extension methods
/// </summary>
public static class DateTimeExtension
{
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

        System.DateTime result = converted.ToUtcKind();

        return result;
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
        if (tzTime.Kind != DateTimeKind.Unspecified) // Q: Is it more expensive to check or just set it?
            tzTime = tzTime.ToUtcKind();

        System.DateTime result = System.TimeZoneInfo.ConvertTimeToUtc(tzTime, tzInfo);

        return result;
    }

    /// <summary>
    /// Calculates the age in hours between the specified date and the current date and time.
    /// </summary>
    /// <param name="fromDateTime">The specified date and time.</param>
    /// <param name="dateTimePrecision"></param>
    /// <param name="utcNow">The current date and time in UTC. If not provided, the current UTC date and time will be used.</param>
    /// <returns>The age in hours.</returns>
    /// <exception cref="NotSupportedException"></exception>
    [Pure]
    public static double ToAge(this System.DateTime fromDateTime, DateTimePrecision dateTimePrecision, System.DateTime? utcNow = null)
    {
        utcNow ??= System.DateTime.UtcNow;
        TimeSpan timeSpan = (utcNow - fromDateTime).Value;

        return dateTimePrecision.Name switch
        {
            nameof(DateTimePrecision.Day) => timeSpan.TotalDays,
            nameof(DateTimePrecision.Hour) => timeSpan.TotalHours,
            nameof(DateTimePrecision.Minute) => timeSpan.TotalMinutes,
            nameof(DateTimePrecision.Second) => timeSpan.TotalSeconds,
            _ => throw new NotSupportedException("DateTimePrecision is not supported for this method")
        };
    }

    /// <summary>
    /// Trims a <see cref="System.DateTime"/> object to a specified level of precision.
    /// </summary>
    /// <remarks>
    /// This method adjusts a <see cref="System.DateTime"/> object to the nearest lower value of the specified precision. For example, trimming to <see cref="DateTimePrecision.Minute"/> 
    /// will result in a <see cref="System.DateTime"/> object set to the beginning of the minute, with seconds and milliseconds set to zero.
    /// The method supports various levels of precision, such as Year, Month, Day, Hour, Minute, and Second. Any time components finer than the specified precision are set to zero.
    /// The resulting <see cref="System.DateTime"/> is always returned with its <see cref="System.DateTime.Kind"/> property set to <see cref="DateTimeKind.Utc"/>.
    /// </remarks>
    /// <param name="dateTime">The <see cref="System.DateTime"/> to trim.</param>
    /// <param name="precision">The precision to which the <paramref name="dateTime"/> should be trimmed. This should be one of the values defined in <see cref="DateTimePrecision"/>.</param>
    /// <param name="dateTimeKind"></param>
    /// <returns>A new <see cref="System.DateTime"/> object trimmed to the specified <paramref name="precision"/>.</returns>
    [Pure]
    public static System.DateTime Trim(this System.DateTime dateTime, DateTimePrecision precision, DateTimeKind? dateTimeKind = null)
    {
        System.DateTime trimmed;

        dateTimeKind ??= dateTime.Kind;

        switch (precision.Name)
        {
            case nameof(DateTimePrecision.Microsecond):
                {
                    long truncatedTicks = dateTime.Ticks - dateTime.Ticks % 10;
                    trimmed = new System.DateTime(truncatedTicks, dateTime.Kind);
                    break;
                }
            case nameof(DateTimePrecision.Millisecond):
                {
                    long truncatedTicks = dateTime.Ticks - dateTime.Ticks % 10000;
                    trimmed = new System.DateTime(truncatedTicks, dateTime.Kind);
                    break;
                }
            case nameof(DateTimePrecision.Second):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Minute):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Hour):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Day):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Month):
                trimmed = new System.DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Quarter):
                // Determine the start month of the quarter
                int quarterNumber = (dateTime.Month - 1) / 3;
                int startMonthOfQuarter = quarterNumber * 3 + 1;
                trimmed = new System.DateTime(dateTime.Year, startMonthOfQuarter, 1, 0, 0, 0, 0, dateTime.Kind);
                break;
            case nameof(DateTimePrecision.Year):
                trimmed = new System.DateTime(dateTime.Year, 1, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            case nameof(DateTimePrecision.Decade):
                // Calculate the start year of the decade
                int startYearOfDecade = dateTime.Year - dateTime.Year % 10;
                trimmed = new System.DateTime(startYearOfDecade, 1, 1, 0, 0, 0, 0, dateTimeKind.Value);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(precision), $"Unsupported precision: {precision.Name}");
        }

        return trimmed;
    }

    /// <summary>
    /// Adjusts the provided <see cref="System.DateTime"/> object to the end of the specified period, minus one tick.
    /// </summary>
    /// <param name="dateTime">The date and time value to adjust.</param>
    /// <param name="precision">The precision level to which the date and time should be adjusted. This determines the period (e.g., Year, Month, Day, etc.) to which the <paramref name="dateTime"/> will be trimmed.</param>
    /// <param name="dateTimeKind">Optional. Specifies the kind of date and time adjustment. If not provided, the kind of <paramref name="dateTime"/> will be used. This can influence the handling of time zones.</param>
    /// <returns>
    /// A new <see cref="System.DateTime"/> object representing the last moment of the specified period, just before it transitions to the next period, according to the specified <paramref name="precision"/>.
    /// </returns>
    /// <remarks>
    /// This method first calculates the start of the next period based on the specified <paramref name="precision"/> and <paramref name="dateTimeKind"/>, if provided. It then subtracts one tick from this calculated start time to get the precise end of the current period.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if an unsupported <paramref name="precision"/> is provided.</exception>
    [Pure]
    public static System.DateTime TrimEnd(this System.DateTime dateTime, DateTimePrecision precision, DateTimeKind? dateTimeKind = null)
    {
        System.DateTime startOfPeriod = dateTime.Trim(precision, dateTimeKind);

        startOfPeriod = precision.Name switch
        {
            nameof(DateTimePrecision.Microsecond) => startOfPeriod.AddTicks(10), // Add 10 ticks to move to the start of the next microsecond
            nameof(DateTimePrecision.Millisecond) => startOfPeriod.AddMilliseconds(1),
            nameof(DateTimePrecision.Second) => startOfPeriod.AddSeconds(1),
            nameof(DateTimePrecision.Minute) => startOfPeriod.AddMinutes(1),
            nameof(DateTimePrecision.Hour) => startOfPeriod.AddHours(1),
            nameof(DateTimePrecision.Day) => startOfPeriod.AddDays(1),
            nameof(DateTimePrecision.Month) => startOfPeriod.AddMonths(1),
            nameof(DateTimePrecision.Quarter) => startOfPeriod.AddMonths(3), // Quarters consist of 3 months
            nameof(DateTimePrecision.Year) => startOfPeriod.AddYears(1),
            nameof(DateTimePrecision.Decade) => startOfPeriod.AddYears(10),
            _ => throw new ArgumentOutOfRangeException(nameof(precision), $"Unsupported precision: {precision.Name}")
        };

        // Subtract one tick to get the last moment of the current period
        return startOfPeriod.AddTicks(-1);
    }

    /// <inheritdoc cref="Trim(System.DateTime, DateTimePrecision, DateTimeKind?)"/>
    [Pure]
    public static System.DateTime ToStartOf(this System.DateTime dateTime, DateTimePrecision dateTimePrecision, DateTimeKind? dateTimeKind = null)
    {
        return Trim(dateTime, dateTimePrecision, dateTimeKind);
    }

    /// <inheritdoc cref="TrimEnd(System.DateTime, DateTimePrecision, DateTimeKind?)"/>
    [Pure]
    public static System.DateTime ToEndOf(this System.DateTime dateTime, DateTimePrecision dateTimePrecision, DateTimeKind? dateTimeKind = null)
    {
        return TrimEnd(dateTime, dateTimePrecision, dateTimeKind);
    }

    /// <summary>
    /// Essentially wraps <see cref="System.DateTime.SpecifyKind"/> in extension method
    /// </summary>
    [Pure]
    public static System.DateTime ToUtcKind(this System.DateTime dateTime)
    {
        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }

    /// <summary>
    /// Essentially wraps <see cref="System.DateTime.SpecifyKind"/> in extension method
    /// </summary>
    [Pure]
    public static System.DateTime ToUnspecifiedKind(this System.DateTime dateTime)
    {
        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    }

    /// <summary>
    /// Converts a <see cref="System.DateTime"/> object to a <see cref="System.DateTimeOffset"/> object.
    /// </summary>
    /// <remarks>
    /// This method creates a <see cref="System.DateTimeOffset"/> object from the provided <see cref="System.DateTime"/>.
    /// The <see cref="System.DateTime"/> object is assumed to be in the local time zone if it is unspecified or specified as local.
    /// If the <see cref="System.DateTime"/> object is specified as UTC, the resulting <see cref="System.DateTimeOffset"/>
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
    /// This method first converts the <see cref="System.DateTime"/> object to a <see cref="System.DateTimeOffset"/> to accurately account for time zone differences
    /// before calculating the Unix time. The input <see cref="System.DateTime"/> should be in UTC to ensure an accurate conversion to Unix time seconds.
    /// </remarks>
    /// <param name="utc">The UTC <see cref="System.DateTime"/> to convert to Unix time seconds.</param>
    /// <returns>The number of seconds that have elapsed since 1970-01-01T00:00:00Z, represented as a long.</returns>
    [Pure]
    public static long ToUnixTimeSeconds(this System.DateTime utc)
    {
        long result = utc.ToDateTimeOffset().ToUnixTimeSeconds();
        return result;
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
        bool result = dateTime >= startAt && dateTime <= endAt;
        return result;
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
        int result = int.Parse(str);
        return result;
    }
}