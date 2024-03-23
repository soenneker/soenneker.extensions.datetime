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
    /// Calculates the age in hours between the specified date and the current date and time.
    /// </summary>
    /// <param name="fromDateTime">The specified date and time.</param>
    /// <param name="dateTimePrecision"></param>
    /// <param name="utc">The current date and time in UTC. If not provided, the current UTC date and time will be used.</param>
    /// <returns>The age in hours.</returns>
    /// <exception cref="NotSupportedException"></exception>
    [Pure]
    public static double ToAge(this System.DateTime fromDateTime, DateTimePrecision dateTimePrecision, System.DateTime? utc = null)
    {
        utc ??= System.DateTime.UtcNow;
        TimeSpan timeSpan = (utc - fromDateTime).Value;

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
    /// The resulting <see cref="System.DateTime"/> is always returned with its <see cref="DateTime.Kind"/> property set to <see cref="DateTimeKind.Utc"/>.
    /// </remarks>
    /// <param name="dateTime">The <see cref="System.DateTime"/> to trim.</param>
    /// <param name="precision">The precision to which the <paramref name="dateTime"/> should be trimmed. This should be one of the values defined in <see cref="DateTimePrecision"/>.</param>
    /// <returns>A new <see cref="System.DateTime"/> object trimmed to the specified <paramref name="precision"/>, with <see cref="DateTime.Kind"/> set to <see cref="DateTimeKind.Utc"/>.</returns>
    [Pure]
    public static System.DateTime Trim(this System.DateTime dateTime, DateTimePrecision precision)
    {
        System.DateTime trimmed = precision.Name switch
        {
            nameof(DateTimePrecision.Second) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second),
            nameof(DateTimePrecision.Minute) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0),
            nameof(DateTimePrecision.Hour) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0),
            nameof(DateTimePrecision.Day) => new System.DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0),
            nameof(DateTimePrecision.Month) => new System.DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0),
            nameof(DateTimePrecision.Year) => new System.DateTime(dateTime.Year, 1, 1, 0, 0, 0),
            _ => dateTime
        };

        return trimmed.ToUtcKind();
    }

    /// <summary>
    /// Not typically for UI display, for admin/debug purposes
    /// </summary>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffffff"</code>
    [Pure]
    public static string ToPreciseDisplay(this System.DateTime dateTime)
    {
        var result = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
        return result;
    }

    /// <summary>
    /// Essentially wraps <see cref="DateTime.SpecifyKind"/> in extension method
    /// </summary>
    [Pure]
    public static System.DateTime ToUtcKind(this System.DateTime dateTime)
    {
        return System.DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
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
    /// yyyy-MM-ddTHH:mm:ss.fffZ. Can be used for Cosmos queries. ISO 8601
    /// </summary>
    /// <param name="utc">Needs to be UTC</param>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffZ"</code>
    [Pure]
    public static string ToIso8601(this System.DateTime utc)
    {
        var result = utc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        return result;
    }

    /// <summary>
    /// Inclusive (returns true if datetime is equal to start or end)
    /// </summary>
    [Pure]
    public static bool IsWithinRange(this System.DateTime dateTime, System.DateTime startAt, System.DateTime endAt)
    {
        bool result = dateTime >= startAt && dateTime <= endAt;
        return result;
    }
}