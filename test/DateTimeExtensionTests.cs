using Soenneker.Enums.DateTimePrecision;
using Soenneker.Tests.Unit;
using Xunit;

namespace Soenneker.Extensions.DateTime.Tests;

public class DateTimeExtensionTests : UnitTest
{
    [Fact]
    public void Trim_should_trim()
    {
        System.DateTime utcNow = System.DateTime.UtcNow;

        System.DateTime result = utcNow.Trim(DateTimePrecision.Minute);
    }
}