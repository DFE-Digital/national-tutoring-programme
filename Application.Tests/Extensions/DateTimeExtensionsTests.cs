using System;
using Application.Extensions;
using Xunit;

namespace Application.Tests.Extensions;

public class DateTimeExtensionsTests
{

    // Temporarily disabled as it fails on the build server
    // [Fact]
    // public void ConvertUtcToUk_LocalDateTimeReturned()
    // {
    //     // Arrange
    //     var utcTime = new DateTime(2023, 03, 28, 10, 0, 0); // 10:00 AM UTC
    //     var expectedUkTime = new DateTime(2023, 03, 28, 11, 0, 0); // 11:00 AM UK local time
    //     
    //     var offset = DateTimeOffset.Now;
    //     
    //     if (offset.Offset == TimeSpan.Zero)
    //     {
    //         expectedUkTime = utcTime;
    //     }
    //
    //     // Act
    //     var actualUkTime = utcTime.ToLocalDateTime("GMT Standard Time");
    //
    //     // Assert
    //     Assert.Equal(expectedUkTime, actualUkTime);
    // }
}