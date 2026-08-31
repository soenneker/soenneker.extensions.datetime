using AwesomeAssertions;
using Soenneker.Enums.UnitOfTime;
using Soenneker.Tests.Unit;
using System;
using System.Globalization;

namespace Soenneker.Extensions.DateTime.Tests;

public class DateTimeExtensionTests : UnitTest
{
    [Test]
    public void Add_nanoseconds_truncates_to_datetime_tick_resolution()
    {
        var value = new System.DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        System.DateTime result = value.Add(150, UnitOfTime.Nanosecond);

        (result.Ticks - value.Ticks).Should().Be(1);
    }

    [Test]
    public void Add_fractional_microseconds_truncates_to_datetime_tick_resolution()
    {
        var value = new System.DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        System.DateTime result = value.Add(0.15, UnitOfTime.Microsecond);

        (result.Ticks - value.Ticks).Should().Be(1);
    }

    [Test]
    public void Trim_quarter_uses_requested_kind()
    {
        var value = new System.DateTime(2024, 5, 20, 12, 0, 0, DateTimeKind.Local);

        System.DateTime result = value.Trim(UnitOfTime.Quarter, DateTimeKind.Utc);

        result.Should().Be(new System.DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc));
    }

    [Test]
    public void ToTzOffset_uses_the_supplied_utc_instant_for_dst()
    {
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var afterDstTransition = new System.DateTime(2023, 3, 12, 7, 30, 0, DateTimeKind.Utc);

        TimeSpan result = afterDstTransition.ToTzOffset(easternZone);

        result.Should().Be(TimeSpan.FromHours(-4));
    }

    [Test]
    public void Add_fractional_quarter_preserves_fractional_months()
    {
        var value = new System.DateTime(2024, 1, 1);

        System.DateTime result = value.Add(0.5, UnitOfTime.Quarter);

        result.Should().Be(new System.DateTime(2024, 2, 15, 12, 0, 0));
    }

    [Test]
    public void ToNextBusinessDate_from_friday_returns_monday_and_preserves_time_and_kind()
    {
        var friday = new System.DateTime(2024, 6, 14, 12, 30, 0, DateTimeKind.Utc);

        System.DateTime result = friday.ToNextBusinessDate(CultureInfo.InvariantCulture);

        result.Should().Be(new System.DateTime(2024, 6, 17, 12, 30, 0, DateTimeKind.Utc));
    }

    [Test]
    public void ToPreviousBusinessDate_from_monday_returns_friday_and_preserves_time_and_kind()
    {
        var monday = new System.DateTime(2024, 6, 17, 12, 30, 0, DateTimeKind.Utc);

        System.DateTime result = monday.ToPreviousBusinessDate(CultureInfo.InvariantCulture);

        result.Should().Be(new System.DateTime(2024, 6, 14, 12, 30, 0, DateTimeKind.Utc));
    }

    [Test]
    public void BusinessDate_methods_support_friday_saturday_weekends()
    {
        CultureInfo culture = CultureInfo.GetCultureInfo("ar-SA");
        var thursday = new System.DateTime(2024, 6, 13, 12, 30, 0, DateTimeKind.Utc);
        var sunday = new System.DateTime(2024, 6, 16, 12, 30, 0, DateTimeKind.Utc);

        thursday.ToNextBusinessDate(culture).Should().Be(sunday);
        sunday.ToPreviousBusinessDate(culture).Should().Be(thursday);
    }

    [Test]
    public void ToAge_returns_negative_calendar_units_for_future_values()
    {
        var now = new System.DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        System.DateTime future = now.AddMonths(2);

        double result = future.ToAge(UnitOfTime.Month, now);

        result.Should().Be(-2);
    }

    [Test]
    public void Trim_should_trim()
    {
        System.DateTime utcNow = System.DateTime.UtcNow;

        System.DateTime result = utcNow.Trim(UnitOfTime.Minute);
        result.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Test]
    public void Should_ConvertToUtc_FromEasternStandardTime()
    {
        // Arrange
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var tzTime = new System.DateTime(2023, 3, 10, 12, 0, 0); // Before DST starts in 2023
        var expectedUtcTime = new System.DateTime(2023, 3, 10, 17, 0, 0, DateTimeKind.Utc); // EST is UTC-5

        // Act
        System.DateTime utcTime = tzTime.ToUtc(easternZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }

    [Test]
    public void Should_HandleDaylightSavingTime_ForEasternTime()
    {
        // Arrange
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var tzTime = new System.DateTime(2023, 3, 13, 12, 0, 0); // After DST starts in 2023
        var expectedUtcTime = new System.DateTime(2023, 3, 13, 16, 0, 0, DateTimeKind.Utc); // EDT is UTC-4

        // Act
        System.DateTime utcTime = tzTime.ToUtc(easternZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }

    [Test]
    public void Should_ConvertToUtc_FromArizonaTimeZone()
    {
        // Arizona does not observe DST
        TimeZoneInfo arizonaZone = TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
        var tzTime = new System.DateTime(2023, 3, 10, 12, 0, 0); // Date doesn't matter as much since no DST
        var expectedUtcTime = new System.DateTime(2023, 3, 10, 19, 0, 0, DateTimeKind.Utc); // MST is UTC-7

        // Act
        System.DateTime utcTime = tzTime.ToUtc(arizonaZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }
}
