using System.Diagnostics.Contracts;
using System.Globalization;
using Soenneker.Extensions.TimeZoneInfos;

namespace Soenneker.Extensions.DateTime;

public static class DateTimeExtensionFormat
{
    /// <summary><code>hh tt {timezone}</code></summary>
    [Pure]
    public static string ToHourFormat(this System.DateTime dateTime, System.TimeZoneInfo timeZoneInfo)
    {
        return dateTime.ToString($"hh tt {timeZoneInfo.ToSimpleAbbreviation()}");
    }

    /// <summary>
    /// Not typically for UI display, for admin/debug purposes
    /// </summary>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffffff"</code>
    [Pure]
    public static string ToPreciseFormat(this System.DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
    }

    /// <summary>"MM-dd-yyyy"</summary>
    [Pure]
    public static string ToMonthFirstDateFormat(this System.DateTime dateTime)
    {
        return dateTime.ToString("MM-dd-yyyy");
    }

    /// <summary>
    /// Not typically for UI display, for admin/debug purposes. Appends Zulu ("Z") to string. Does not do any conversion.
    /// </summary>
    /// <param name="utc">Needs to be in UTC already.</param>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffffffZ"</code>
    [Pure]
    public static string ToPreciseUtcFormat(this System.DateTime utc)
    {
        return utc.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
    }

    /// <summary>
    /// yyyy-MM-ddTHH:mm:ss.fffZ. ISO 8601. Can be used for Cosmos queries.
    /// </summary>
    /// <param name="utc">Needs to be UTC</param>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffZ"</code>
    [Pure]
    public static string ToIso8601(this System.DateTime utc)
    {
        return utc.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
    }

    ///<inheritdoc cref="ToIso8601"/>
    [Pure]
    public static string ToWebString(this System.DateTime utc)
    {
        return utc.ToIso8601();
    }

    /// <summary>
    /// Converts UTC time into Eastern, and then appends a timezone display (i.e. 'ET') <para/>
    /// <code>MM/dd/yyyy hh:mm:ss tt ET</code>
    /// </summary>
    /// <param name="utcTime">Needs to be UTC</param>
    /// <param name="tzInfo"></param>
    [Pure]
    public static string ToTzDateTimeFormat(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        return utcTime.ToTz(tzInfo).ToDateTimeFormatAsTz(tzInfo);
    }

    /// <summary>
    /// Converts UTC time into Tz first<para/>
    /// <code>MM/dd/yyyy</code>
    /// </summary>
    /// <param name="utcTime">Needs to be UTC</param>
    /// <param name="tzInfo"></param>
    [Pure]
    public static string ToTzDateFormat(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        return utcTime.ToTz(tzInfo).ToString("MM/dd/yyyy");
    }

    /// <summary>
    /// Essentially <see cref="ToTzDateTimeFormat"/> but doesn't include minutes or seconds <para/>
    /// <code>MM/dd/yyyy h tt ET</code>
    /// </summary>
    /// <param name="utcTime">Needs to be UTC</param>
    /// <param name="tzInfo"></param>
    [Pure]
    public static string ToTzDateHourFormat(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        return utcTime.ToTz(tzInfo).ToString($"MM/dd/yyyy h tt {tzInfo.ToSimpleAbbreviation()}");
        ;
    }

    /// <summary>
    /// Does NOT convert to Tz. Formats and appends a timezone display (i.e. 'ET') <para/>
    /// <code>MM/dd/yyyy hh:mm:ss tt ET</code>
    /// </summary>
    [Pure]
    public static string ToDateTimeFormatAsTz(this System.DateTime tzTime, System.TimeZoneInfo tzInfo)
    {
        return tzTime.ToString($"MM/dd/yyyy hh:mm:ss tt {tzInfo.ToSimpleAbbreviation()}");
    }

    /// <summary>
    /// Does NOT convert.<para/>
    /// <code>MM/dd/yyyy hh:mm:ss tt UTC</code>
    /// </summary>
    /// <param name="utc">Needs to be UTC</param>
    [Pure]
    public static string ToUtcDateTimeFormat(this System.DateTime utc)
    {
        return utc.ToString("MM/dd/yyyy hh:mm:ss tt UTC");
    }

    /// <summary>
    /// Converts to tzTime and then formats <para/>
    /// <code>yyyy-MM-dd--HH-mm-ss</code>
    /// </summary>
    [Pure]
    public static string ToTzFileName(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        return utcTime.ToTz(tzInfo).ToFileName();
    }

    /// <summary>
    /// Simply formats <para/>
    /// <code>yyyy-MM-dd--HH-mm-ss</code>
    /// </summary>
    [Pure]
    public static string ToFileName(this System.DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd--HH-mm-ss");
    }

    /// <summary>
    /// Formats the DateTime object to a string in the format "MMM dd, yyyy".
    /// </summary>
    /// <param name="dateTime">The DateTime object to format.</param>
    /// <returns>A string representing the formatted DateTime in the "MMM dd, yyyy" format.</returns>
    /// <example>
    /// <code>
    /// DateTime date = new DateTime(2017, 1, 5);
    /// string formattedDate = date.ToDisplayFormat();
    /// // formattedDate will be "Jan 05, 2017"
    /// </code>
    /// </example>
    [Pure]
    public static string ToShortMonthDayYearString(this System.DateTime dateTime)
    {
        return dateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Formats the DateTime object to a string in the format "MMMM d, yyyy".
    /// </summary>
    /// <param name="dateTime">The DateTime object to format.</param>
    /// <returns>A string representing the formatted DateTime in the "MMMM d, yyyy" format.</returns>
    /// <example>
    /// <code>
    /// DateTime date = new DateTime(2017, 1, 5);
    /// string formattedDate = date.ToLongMonthDayYearString();
    /// // formattedDate will be "January 5, 2017"
    /// </code>
    /// </example>
    [Pure]
    public static string ToLongMonthDayYearString(this System.DateTime dateTime)
    {
        return dateTime.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
    }
}