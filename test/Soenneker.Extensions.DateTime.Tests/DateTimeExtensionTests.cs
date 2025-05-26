using AwesomeAssertions;
using Soenneker.Enums.UnitOfTime;
using Soenneker.Tests.Unit;
using System;
using Xunit;

namespace Soenneker.Extensions.DateTime.Tests;

public class DateTimeExtensionTests : UnitTest
{
    [Fact]
    public void Trim_should_trim()
    {
        System.DateTime utcNow = System.DateTime.UtcNow;

        System.DateTime result = utcNow.Trim(UnitOfTime.Minute);
        result.Kind.Should().Be(System.DateTimeKind.Utc);
    }

    [Fact]
    public void Should_ConvertToUtc_FromEasternStandardTime()
    {
        // Arrange
        System.TimeZoneInfo easternZone = System.TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var tzTime = new System.DateTime(2023, 3, 10, 12, 0, 0); // Before DST starts in 2023
        var expectedUtcTime = new System.DateTime(2023, 3, 10, 17, 0, 0, DateTimeKind.Utc); // EST is UTC-5

        // Act
        System.DateTime utcTime = tzTime.ToUtc(easternZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }

    [Fact]
    public void Should_HandleDaylightSavingTime_ForEasternTime()
    {
        // Arrange
        System.TimeZoneInfo easternZone = System.TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        var tzTime = new System.DateTime(2023, 3, 13, 12, 0, 0); // After DST starts in 2023
        var expectedUtcTime = new System.DateTime(2023, 3, 13, 16, 0, 0, DateTimeKind.Utc); // EDT is UTC-4

        // Act
        System.DateTime utcTime = tzTime.ToUtc(easternZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }

    [Fact]
    public void Should_ConvertToUtc_FromArizonaTimeZone()
    {
        // Arizona does not observe DST
        System.TimeZoneInfo arizonaZone = System.TimeZoneInfo.FindSystemTimeZoneById("US Mountain Standard Time");
        var tzTime = new System.DateTime(2023, 3, 10, 12, 0, 0); // Date doesn't matter as much since no DST
        var expectedUtcTime = new System.DateTime(2023, 3, 10, 19, 0, 0, DateTimeKind.Utc); // MST is UTC-7

        // Act
        System.DateTime utcTime = tzTime.ToUtc(arizonaZone);

        // Assert
        utcTime.Should().Be(expectedUtcTime);
    }
}