﻿// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Attrify.Attributes;
using Attrify.InvisibleApi.Models;
using Attrify.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Moq;

namespace Attrify.InvisibleApi.Tests.Unit.Middlewares
{
    public partial class InvisibleApiMiddlewareTests
    {
        [Fact]
        public async Task ShouldHitApiEndpointIfEndpointIsntConfiguredAsInvisible()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
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
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(contextMock.Object);

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
        public async Task ShouldHitApiEndpointIfEndpointIsConfiguredProperly()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
            string randomEndpoint = $"/{GetRandomString()}";
            string randomHttpVerb = GetRandomString();
            var requestDelegateMock = new Mock<RequestDelegate>();
            var contextMock = new Mock<HttpContext>();
            var featuresMock = new Mock<IFeatureCollection>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
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

            var httpRequestMock = new Mock<HttpRequest>();
            var httpResponseMock = new Mock<HttpResponse>();

            httpRequestMock.SetupGet(request => request.Path)
                .Returns(randomEndpoint);

            httpRequestMock.SetupGet(request => request.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
                    { randomInvisibleApiKey.Key, randomInvisibleApiKey.Value}
                }));

            httpRequestMock.SetupGet(request => request.Method)
                .Returns(randomHttpVerb);

            contextMock.SetupGet(context => context.Request)
                .Returns(httpRequestMock.Object);

            contextMock.SetupGet(context => context.Response)
                .Returns(httpResponseMock.Object);

            var identityMock = new Mock<IIdentity>();
            identityMock.Setup(id => id.IsAuthenticated).Returns(true);

            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

            claimsPrincipalMock.Setup(user =>
                user.Identity)
                    .Returns(identityMock.Object);

            claimsPrincipalMock.Setup(user =>
                user.IsInRole(randomInvisibleApiKey.Key))
                    .Returns(true);

            contextMock.Setup(context =>
                context.User)
                    .Returns(claimsPrincipalMock.Object);

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(contextMock.Object);

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

            httpRequestMock.VerifyGet(request =>
                request.Headers,
                    Times.Once());

            claimsPrincipalMock.Verify(user =>
                user.IsInRole(randomInvisibleApiKey.Key),
                    Times.Once);

            identityMock.Verify(id =>
                id.IsAuthenticated,
                    Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
            contextMock.VerifyNoOtherCalls();
            featuresMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
            httpRequestMock.VerifyNoOtherCalls();
            httpResponseMock.VerifyNoOtherCalls();
            identityMock.VerifyNoOtherCalls();
            claimsPrincipalMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNotFoundIfHeaderValueDontMatchInvisibleConfiguration()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
            string randomHeaderName = GetRandomString();
            string randomHeaderValue = GetRandomString();
            string randomEndpoint = $"/{GetRandomString()}";
            string randomHttpVerb = GetRandomString();
            var requestDelegateMock = new Mock<RequestDelegate>();
            var contextMock = new Mock<HttpContext>();
            var featuresMock = new Mock<IFeatureCollection>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
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

            var httpRequestMock = new Mock<HttpRequest>();
            var httpResponseMock = new Mock<HttpResponse>();

            httpRequestMock.SetupGet(request => request.Path)
                .Returns(randomEndpoint);

            httpRequestMock.SetupGet(request => request.Headers)
                .Returns(new HeaderDictionary(new Dictionary<string, StringValues>
                {
            { randomHeaderName, randomHeaderValue }
                }));

            httpRequestMock.SetupGet(request => request.Method)
                .Returns(randomHttpVerb);

            contextMock.SetupGet(context => context.Request)
                .Returns(httpRequestMock.Object);

            contextMock.SetupGet(context => context.Response)
                .Returns(httpResponseMock.Object);

            var identityMock = new Mock<IIdentity>();
            identityMock.Setup(id => id.IsAuthenticated).Returns(true);

            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

            claimsPrincipalMock.Setup(user =>
                user.Identity)
                    .Returns(identityMock.Object);

            claimsPrincipalMock.Setup(user =>
                user.IsInRole(randomInvisibleApiKey.Key))
                    .Returns(true);

            contextMock.Setup(context =>
                context.User)
                    .Returns(claimsPrincipalMock.Object);

            httpResponseMock.SetupSet(response => response.StatusCode = It.IsAny<int>())
                .Verifiable();

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(contextMock.Object);

            // then
            httpResponseMock.VerifySet(response =>
                response.StatusCode = StatusCodes.Status404NotFound, Times.Once);

            featuresMock.Verify(features =>
                features.Get<IEndpointFeature>(),
                    Times.Once);

            endpointFeatureMock.Verify(endpointFeature =>
                endpointFeature.Endpoint,
                    Times.Once);

            httpRequestMock.VerifyGet(request =>
                request.Headers,
                    Times.Once());

            claimsPrincipalMock.Verify(user =>
                user.IsInRole(randomInvisibleApiKey.Key),
                    Times.Once);

            identityMock.Verify(id =>
                id.IsAuthenticated,
                    Times.Once);

            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(contextMock.Object),
                    Times.Never);

            requestDelegateMock.VerifyNoOtherCalls();
            contextMock.VerifyNoOtherCalls();
            featuresMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
            httpRequestMock.VerifyNoOtherCalls();
            httpResponseMock.VerifyNoOtherCalls();
            identityMock.VerifyNoOtherCalls();
            claimsPrincipalMock.VerifyNoOtherCalls();
        }
    }
}
