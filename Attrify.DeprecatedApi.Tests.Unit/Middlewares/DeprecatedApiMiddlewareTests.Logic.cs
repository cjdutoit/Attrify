// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Attrify.Attributes;
using Attrify.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Moq;

namespace Attrify.InvisibleApi.Tests.Unit.Middlewares
{
    public partial class DeprecatedApiMiddlewareTests
    {
        [Fact]
        public async Task ShouldHitApiEndpointIfEndpointIsntConfiguredAsDeprecated()
        {
            // given
            var requestDelegateMock = new Mock<RequestDelegate>();
            var contextMock = new Mock<HttpContext>();
            var featuresMock = new Mock<IFeatureCollection>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(),
                displayName: "TestEndpoint");

            endpointFeatureMock.Setup(endpointFeature =>
                endpointFeature.Endpoint)
                    .Returns(endpoint);

            featuresMock.Setup(features =>
                features.Get<IEndpointFeature>())
                    .Returns(endpointFeatureMock.Object);

            contextMock.Setup(context =>
                context.Features)
                    .Returns(featuresMock.Object);

            // when
            var deprecatedApiMiddleware = new DeprecatedApiMiddleware(
                next: requestDelegateMock.Object);

            await deprecatedApiMiddleware.InvokeAsync(contextMock.Object);

            // then
            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(contextMock.Object),
                    Times.Once);

            featuresMock.Verify(features =>
                features.Get<IEndpointFeature>(),
                    Times.Once);

            endpointFeatureMock.Verify(endpointFeature =>
                endpointFeature.Endpoint,
                    Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
            contextMock.VerifyNoOtherCalls();
            featuresMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldMarkApiAsDeprecatedButNotExpired()
        {
            // given
            var requestDelegateMock = new Mock<RequestDelegate>();
            var contextMock = new Mock<HttpContext>();
            var featuresMock = new Mock<IFeatureCollection>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();
            var futureSunsetDate = DateTime.UtcNow.AddDays(30).ToString("yyyy-MM-dd");

            var deprecatedApiAttribute = new DeprecatedApiAttribute
            {
                Sunset = futureSunsetDate,
                Warning = $"This API is deprecated and will be removed on {futureSunsetDate}.",
                Link = "https://api.example.com/deprecation-info"
            };

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(deprecatedApiAttribute),
                displayName: "TestEndpoint");

            endpointFeatureMock.Setup(endpointFeature =>
                endpointFeature.Endpoint)
                .Returns(endpoint);

            featuresMock.Setup(features =>
                features.Get<IEndpointFeature>())
                .Returns(endpointFeatureMock.Object);

            contextMock.Setup(context =>
                context.Features)
                .Returns(featuresMock.Object);

            var responseMock = new Mock<HttpResponse>();
            var headersMock = new HeaderDictionary();
            responseMock.Setup(response => response.Headers).Returns(headersMock);
            contextMock.Setup(context => context.Response).Returns(responseMock.Object);

            // when
            var deprecatedApiMiddleware = new DeprecatedApiMiddleware(
                next: requestDelegateMock.Object);

            await deprecatedApiMiddleware.InvokeAsync(contextMock.Object);

            // then
            contextMock.Object.Response.Headers.ContainsKey("Sunset").Should().BeTrue();

            contextMock.Object.Response.Headers["Sunset"].ToString()
                .Should().Be(futureSunsetDate);

            contextMock.Object.Response.Headers.ContainsKey("Warning").Should().BeTrue();

            contextMock.Object.Response.Headers["Warning"].ToString()
                .Should().Be($"{deprecatedApiAttribute.Warning}");

            contextMock.Object.Response.Headers.ContainsKey("Link").Should().BeTrue();

            contextMock.Object.Response.Headers["Link"].ToString()
                .Should().Be($"<{deprecatedApiAttribute.Link}>; rel=\"deprecation\"");

            contextMock.Object.Response.StatusCode.Should().NotBe(StatusCodes.Status410Gone);

            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(contextMock.Object),
                    Times.Once);

            featuresMock.Verify(features =>
                features.Get<IEndpointFeature>(),
                    Times.Once);

            endpointFeatureMock.Verify(endpointFeature =>
                endpointFeature.Endpoint,
                    Times.Once);

            contextMock.Verify(context =>
                context.Response.Headers,
                    Times.AtLeastOnce);

            contextMock.Verify(context =>
                context.Response.StatusCode,
                    Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
            contextMock.VerifyNoOtherCalls();
            featuresMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldMarkApiAsDeprecatedAndReturnStatus410GoneOnSunset()
        {
            // Given
            var requestDelegateMock = new Mock<RequestDelegate>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();
            var pastSunsetDate = DateTime.UtcNow.ToString("yyyy-MM-dd");

            var deprecatedApiAttribute = new DeprecatedApiAttribute
            {
                Sunset = pastSunsetDate,
                Warning = $"This API is deprecated and will be removed on {pastSunsetDate}.",
                Link = "https://api.example.com/deprecation-info"
            };

            var expectedErrorDetailsDictionary = new Dictionary<string, object>
            {
                { "statusCode", StatusCodes.Status410Gone },
                { "type", "https://tools.ietf.org/html/rfc7231#section-6.5.9" },
                { "title", "Deprecated API" },

                { "error", $"This API has been sunset and is no longer available.  " +
                    $"The link should provide details about alternatives, or migration steps." },

                { "sunsetDate", pastSunsetDate },
                { "link", deprecatedApiAttribute.Link ?? "N/A" }
            };

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(deprecatedApiAttribute),
                displayName: "TestEndpoint");

            endpointFeatureMock.Setup(endpointFeature =>
                endpointFeature.Endpoint)
                .Returns(endpoint);

            var context = new DefaultHttpContext();
            context.Features.Set(endpointFeatureMock.Object);
            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // When
            var deprecatedApiMiddleware = new DeprecatedApiMiddleware(next: requestDelegateMock.Object);
            await deprecatedApiMiddleware.InvokeAsync(context);

            // Then
            context.Response.Headers.ContainsKey("Sunset").Should().BeTrue();
            context.Response.Headers.ContainsKey("Warning").Should().BeTrue();
            context.Response.Headers.ContainsKey("Link").Should().BeTrue();
            context.Response.StatusCode.Should().Be(StatusCodes.Status410Gone);
            context.Response.ContentType.Should().Contain("application/json");

            context.Response.Headers["Sunset"].ToString()
                .Should().Be(pastSunsetDate);

            context.Response.Headers["Warning"].ToString()
                .Should().Be(deprecatedApiAttribute.Warning);

            context.Response.Headers["Link"].ToString()
                .Should().Be($"<{deprecatedApiAttribute.Link}>; rel=\"deprecation\"");

            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            string writtenContent = await reader.ReadToEndAsync();
            var actualErrorDetailsRaw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(writtenContent);

            var actualErrorDetails = actualErrorDetailsRaw.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ValueKind == JsonValueKind.Number
                    ? (object)kvp.Value.GetInt32()
                    : (object)kvp.Value.GetString()
            );

            actualErrorDetails.Should().BeEquivalentTo(expectedErrorDetailsDictionary);

            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(context),
                Times.Never);

            endpointFeatureMock.Verify(endpointFeature =>
                endpointFeature.Endpoint,
                Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
        }
    }
}
