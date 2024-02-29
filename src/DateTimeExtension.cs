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
}