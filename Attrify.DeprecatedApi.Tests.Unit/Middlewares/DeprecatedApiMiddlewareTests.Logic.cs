// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
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
            var futureSunsetDate = DateTime.UtcNow.AddDays(30);

            var deprecatedApiAttribute = new DeprecatedApiAttribute
            {
                Sunset = futureSunsetDate,
                Warning = $"This API is deprecated and will be removed on {futureSunsetDate.ToString("yyyy-MM-dd")}.",
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
                .Should().Be(futureSunsetDate.ToString("yyyy-MM-dd"));

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
    }
}
