// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Threading.Tasks;
using Attrify.Middlewares;
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
    }
}
