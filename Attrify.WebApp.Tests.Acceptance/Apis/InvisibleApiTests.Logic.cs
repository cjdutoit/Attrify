// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;

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
    }
}
