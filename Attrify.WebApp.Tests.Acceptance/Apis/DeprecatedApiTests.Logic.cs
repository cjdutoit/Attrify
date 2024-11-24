// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Attrify.WebApp.Tests.Acceptance.Apis
{
    public partial class DeprecatedApiTests
    {
        [Fact]
        public async Task ShouldGetMessageFromActiveApiAsync()
        {
            // given

            string expectedMessage =
                "This is an active API that is NOT configured for deprecation.";

            // when
            HttpResponseMessage response = await this.apiBroker.httpClient
                .GetAsync(this.apiBroker.NonDeprecatedRelativeUrl);

            string actualMessage = await response.Content.ReadAsStringAsync();

            // then
            actualMessage.Should().Be(expectedMessage);

            bool hasSunsetHeader = response.Headers.Contains("Sunset");
            hasSunsetHeader.Should().BeFalse("because it should not include a 'Sunset' header not deprecated.");

            bool hasWarningHeader = response.Headers.Contains("Warning");
            hasSunsetHeader.Should().BeFalse("because it should not include a 'Warning' header not deprecated.");
        }

        [Fact]
        public async Task ShouldGetMessageFromPendingDeprecationApiAsync()
        {
            // given

            string expectedMessage =
                "This is API has been flagged for deprecation. " +
                "This would be a normal api result, but you will see the deprecation headers";

            string expectedSunset = "2084-11-22";

            string expectedWarning =
                "This API will sunset on 2084-11-22. " +
                "The link should provide details about alternatives, or migration steps.";

            string expectedLink = "https://api.example.com/deprecation-info";

            // when
            HttpResponseMessage response = await this.apiBroker.httpClient
                .GetAsync(this.apiBroker.PendingDeprecatedRelativeUrl);

            string actualMessage = await response.Content.ReadAsStringAsync();

            // then
            actualMessage.Should().Be(expectedMessage);

            response.Headers.TryGetValues("Sunset", out var sunsetValues)
                .Should().BeTrue("because the response should include a 'Sunset' header");

            sunsetValues.Should().ContainSingle()
                .Which.Should().Be(expectedSunset);

            response.Headers.TryGetValues("Warning", out var warningValues)
                .Should().BeTrue("because the response should include a 'Warning' header");

            warningValues.Should().ContainSingle()
                .Which.Should().Be(expectedWarning);

            response.Headers.TryGetValues("Link", out var linkValues)
                .Should().BeTrue("because the response should include a 'Link' header");

            linkValues.Should().ContainSingle()
                .Which.Should().Contain(expectedLink);
        }

        [Fact]
        public async Task ShouldGetGoneStatusFromDeprecationApiAsync()
        {
            // given
            string expectedSunset = "2022-11-22";
            string expectedLink = "https://api.example.com/deprecation-info";

            string expectedWarning =
                $"This API will sunset on 2022-11-22. " +
                $"The link should provide details about alternatives, or migration steps.";

            var errorDetails = new
            {
                StatusCode = StatusCodes.Status410Gone,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.9",
                Title = "Deprecated API",

                Error =
                    $"This API has been sunset and is no longer available.  " +
                    $"The link should provide details about alternatives, or migration steps.",

                SunsetDate = expectedSunset,
                Link = expectedLink
            };

            string expectedErrorDetails = JsonSerializer.Serialize(errorDetails);

            // when
            HttpResponseMessage response = await this.apiBroker.httpClient
                .GetAsync(this.apiBroker.DeprecatedRelativeUrl);

            string responseContent = await response.Content.ReadAsStringAsync();
            string actualErrorDetails = JsonSerializer.Serialize(errorDetails);

            // then
            response.StatusCode.Should().Be(
                HttpStatusCode.Gone, "because the API is deprecated and should return a 410 status code");

            actualErrorDetails.Should().Be(expectedErrorDetails);

            response.Headers.TryGetValues("Sunset", out var sunsetValues)
                .Should().BeTrue("because the response should include a 'Sunset' header");

            sunsetValues.Should().ContainSingle()
                .Which.Should().Be(expectedSunset);

            response.Headers.TryGetValues("Warning", out var warningValues)
                .Should().BeTrue("because the response should include a 'Warning' header");

            warningValues.Should().ContainSingle()
                .Which.Should().Be(expectedWarning);

            response.Headers.TryGetValues("Link", out var linkValues)
                .Should().BeTrue("because the response should include a 'Link' header");

            linkValues.Should().ContainSingle()
                .Which.Should().Contain(expectedLink);
        }
    }
}
