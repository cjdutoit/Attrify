// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using FluentAssertions;
using Moq;

namespace Attrify.DeprecatedApi.Tests.Unit.Services.Foundations
{
    public partial class DeprecatedApiScannerServiceTests
    {
        [Fact]
        public async Task ShouldReturnDeprecatedApisAsync()
        {
            // given
            string sunsetDateString = "2022-11-22";
            DateTimeOffset sunsetDateTimeOffset = DateTimeOffset.Parse(sunsetDateString);

            var expectedDeprecatedApis = new List<DeprecatedApiInfo>
            {
                new DeprecatedApiInfo
                {
                    Controller = "Test",
                    Action = "DeprecatedAction",
                    HttpMethods = new List<string> { "GET" },
                    Route = "api/test/deprecated",
                    Sunset = sunsetDateString,
                    Warning = "This API is deprecated.",
                    Link = "https://api.example.com/deprecation-info",
                    IsDeprecated = true
                }
            };

            dateTimeBrokerMock.Setup(broker => broker.GetCurrentDateTimeOffsetAsync())
                .ReturnsAsync(sunsetDateTimeOffset);

            // when
            var actualDeprecatedApis = await deprecatedApiScannerService.GetDeprecatedApisAsync();

            // then
            actualDeprecatedApis.Should().BeEquivalentTo(expectedDeprecatedApis);
        }
    }
}
