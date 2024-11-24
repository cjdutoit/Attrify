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
    public partial class InvisibleApiTests
    {
        [Fact]
        public async Task ShouldGetMessageNonInvisibleApiAsync()
        {
            // given
            string expectedMessage =
                "This is a normal API.";

            // when
            HttpResponseMessage response = await this.apiBroker.httpClient
                .GetAsync(this.apiBroker.VisibleRelativeUrl);

            string actualMessage = await response.Content.ReadAsStringAsync();

            // then
            actualMessage.Should().Be(expectedMessage);

            bool hasSunsetHeader = response.Headers.Contains("Sunset");
            hasSunsetHeader.Should().BeFalse("because it should not include a 'Sunset' header not deprecated.");

            bool hasWarningHeader = response.Headers.Contains("Warning");
            hasSunsetHeader.Should().BeFalse("because it should not include a 'Warning' header not deprecated.");
        }

        [Fact]
        public async Task ShouldGetNotFoundForInvisibleWithNoHeadersOrRolesApiAsync()
        {
            // given
            var errorDetails = new
            {
                StatusCode = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = $"{StatusCodes.Status404NotFound} -  Not Found",
                Error = $"The requested resource could not be found. Please check the URL and try again.",
            };

            string expectedErrorMessage = JsonSerializer.Serialize(errorDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // when
            HttpResponseMessage response = await this.apiBroker.httpClient
                .GetAsync(this.apiBroker.InvisibleRelativeUrl);

            string actualErrorMessage = await response.Content.ReadAsStringAsync();

            // then
            response.StatusCode.Should().Be(HttpStatusCode.NotFound, "because the API is invisible");
            actualErrorMessage.Should().Be(expectedErrorMessage);
        }

    }
}
