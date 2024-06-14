using Soenneker.Extensions.TimeZoneInfo;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace Soenneker.Extensions.DateTime;

public static class DateTimeExtensionFormat
{
    /// <summary><code>hh tt {timezone}</code></summary>
    [Pure]
    public static string ToHourFormat(this System.DateTime dateTime, System.TimeZoneInfo timeZoneInfo)
    {
        var result = dateTime.ToString($"hh tt {timeZoneInfo.ToSimpleAbbreviation()}");
        return result;
    }

    /// <summary>
    /// Not typically for UI display, for admin/debug purposes
    /// </summary>
    /// <code>"yyyy-MM-ddTHH:mm:ss.fffffff"</code>
    [Pure]
    public static string ToPreciseFormat(this System.DateTime dateTime)
    {
        var result = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
        return result;
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
        string result = utc.ToIso8601();
        return result;
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
        string result = utcTime.ToTz(tzInfo).ToDateTimeFormatAsTz(tzInfo);
        return result;
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
        var result = utcTime.ToTz(tzInfo).ToString("MM/dd/yyyy");
        return result;
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
        var result = utcTime.ToTz(tzInfo).ToString($"MM/dd/yyyy h tt {tzInfo.ToSimpleAbbreviation()}");
        return result;
    }

    /// <summary>
    /// Does NOT convert to Tz. Formats and appends a timezone display (i.e. 'ET') <para/>
    /// <code>MM/dd/yyyy hh:mm:ss tt ET</code>
    /// </summary>
    [Pure]
    public static string ToDateTimeFormatAsTz(this System.DateTime tzTime, System.TimeZoneInfo tzInfo)
    {
        var result = tzTime.ToString($"MM/dd/yyyy hh:mm:ss tt {tzInfo.ToSimpleAbbreviation()}");
        return result;
    }

    /// <summary>
    /// Does NOT convert.<para/>
    /// <code>MM/dd/yyyy hh:mm:ss tt UTC</code>
    /// </summary>
    /// <param name="utc">Needs to be UTC</param>
    [Pure]
    public static string ToUtcDateTimeFormat(this System.DateTime utc)
    {
        var result = utc.ToString("MM/dd/yyyy hh:mm:ss tt UTC");
        return result;
    }

    /// <summary>
    /// Converts to tzTime and then formats <para/>
    /// <code>yyyy-MM-dd--HH-mm-ss</code>
    /// </summary>
    [Pure]
    public static string ToTzFileName(this System.DateTime utcTime, System.TimeZoneInfo tzInfo)
    {
        string result = utcTime.ToTz(tzInfo).ToFileName();
        return result;
    }

    /// <summary>
    /// Simply formats <para/>
    /// <code>yyyy-MM-dd--HH-mm-ss</code>
    /// </summary>
    [Pure]
    public static string ToFileName(this System.DateTime dateTime)
    {
        var result = dateTime.ToString("yyyy-MM-dd--HH-mm-ss");
        return result;
    }

    /// <summary>
    /// Formats the DateTime object to a string in the format "MMM dd, yyyy".
    /// </summary>
    /// <param name="dateTime">The DateTime object to format.</param>
    /// <returns>A string representing the formatted DateTime.</returns>
    public static string ToDisplayFormat(this System.DateTime dateTime)
    {
        return dateTime.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
    }
}