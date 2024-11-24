// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using Attrify.Attributes;
using Attrify.InvisibleApi.Models;
using Attrify.Middlewares;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
            var endpointFeatureMock = new Mock<IEndpointFeature>();
            string expectedResult = "This is a normal API.";

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(),
                displayName: "TestEndpoint");

            endpointFeatureMock.Setup(endpointFeature =>
                endpointFeature.Endpoint)
                    .Returns(endpoint);

            var context = new DefaultHttpContext();
            context.Features.Set(endpointFeatureMock.Object);

            requestDelegateMock
                .Setup(requestDelegate => requestDelegate(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(ctx =>
                {
                    ctx.Response.WriteAsync(expectedResult).Wait();
                })
                .Returns(Task.CompletedTask);

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(context);

            // then
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            string actualResult = await reader.ReadToEndAsync();
            actualResult.Should().Be(expectedResult);

            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(context),
                Times.Once);

            endpointFeatureMock.Verify(endpointFeature =>
                endpointFeature.Endpoint,
                Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
            memoryStream.Dispose();
        }

        [Fact]
        public async Task ShouldHitApiEndpointIfEndpointIsConfiguredProperly()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
            string randomEndpoint = $"/{GetRandomString()}";
            string randomHttpVerb = GetRandomString();
            var requestDelegateMock = new Mock<RequestDelegate>();
            var endpointFeatureMock = new Mock<IEndpointFeature>();
            string expectedResponseContent = "Endpoint hit successfully.";

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
                displayName: "TestEndpoint");

            var context = new DefaultHttpContext();
            context.Features.Set(endpointFeatureMock.Object);

            context.Request.Path = randomEndpoint;
            context.Request.Method = randomHttpVerb;
            context.Request.Headers.Add(randomInvisibleApiKey.Key, randomInvisibleApiKey.Value);

            var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Role, randomInvisibleApiKey.Key)
                },
                "TestAuthentication");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            requestDelegateMock
                .Setup(requestDelegate => requestDelegate(It.IsAny<HttpContext>()))
                .Callback<HttpContext>(ctx =>
                {
                    ctx.Response.WriteAsync(expectedResponseContent).Wait();
                })
                .Returns(Task.CompletedTask);

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(context);

            // then
            memoryStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            string actualResponseContent = await reader.ReadToEndAsync();
            actualResponseContent.Should().Be(expectedResponseContent);

            requestDelegateMock.Verify(requestDelegate =>
                requestDelegate(context),
                Times.Once);

            requestDelegateMock.VerifyNoOtherCalls();
            endpointFeatureMock.VerifyNoOtherCalls();
            memoryStream.Dispose();
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
            string expectedHeaderKey = randomInvisibleApiKey.Key;

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
                displayName: "TestEndpoint");

            var context = new DefaultHttpContext();
            context.Request.Path = randomEndpoint;
            context.Request.Method = randomHttpVerb;
            context.Request.Headers.Add(randomHeaderName, randomHeaderValue);

            var claimsIdentity = new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.Role, expectedHeaderKey) },
                "TestAuthentication");

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;

            var endpointFeatureMock = new Mock<IEndpointFeature>();
            endpointFeatureMock.Setup(feature => feature.Endpoint).Returns(endpoint);
            context.Features.Set(endpointFeatureMock.Object);

            var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(context);

            // then
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            endpointFeatureMock.Verify(feature => feature.Endpoint, Times.Once);
            requestDelegateMock.Verify(requestDelegate => requestDelegate(context), Times.Never);
            memoryStream.Dispose();
        }

        [Fact]
        public async Task ShouldReturnNotFoundIfUserDontHaveMatchingInvisibleApiRole()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
            string randomEndpoint = $"/{GetRandomString()}";
            string randomHttpVerb = GetRandomString();
            var requestDelegateMock = new Mock<RequestDelegate>();

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
                displayName: "TestEndpoint");

            var context = new DefaultHttpContext();
            context.Request.Path = randomEndpoint;
            context.Request.Method = randomHttpVerb;
            context.Request.Headers.Add(randomInvisibleApiKey.Key, randomInvisibleApiKey.Value);

            var claimsIdentity = new ClaimsIdentity(authenticationType: "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;

            var endpointFeatureMock = new Mock<IEndpointFeature>();
            endpointFeatureMock.Setup(feature => feature.Endpoint).Returns(endpoint);
            context.Features.Set(endpointFeatureMock.Object);

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(context);

            // then
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            endpointFeatureMock.Verify(feature => feature.Endpoint, Times.Once);
            requestDelegateMock.Verify(requestDelegate => requestDelegate(context), Times.Never);
        }

        [Fact]
        public async Task ShouldReturnNotFoundIfNeitherHeaderOrUserRoleFound()
        {
            // given
            var randomInvisibleApiKey = new InvisibleApiKey();
            string randomHeaderName = GetRandomString();
            string randomHeaderValue = GetRandomString();
            string randomEndpoint = $"/{GetRandomString()}";
            string randomHttpVerb = GetRandomString();
            var requestDelegateMock = new Mock<RequestDelegate>();

            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new InvisibleApiAttribute()),
                displayName: "TestEndpoint");

            var context = new DefaultHttpContext();
            context.Request.Path = randomEndpoint;
            context.Request.Method = randomHttpVerb;
            context.Request.Headers.Add(randomHeaderName, randomHeaderValue);

            var claimsIdentity = new ClaimsIdentity(authenticationType: "TestAuthentication");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            context.User = claimsPrincipal;

            var endpointFeatureMock = new Mock<IEndpointFeature>();
            endpointFeatureMock.Setup(feature => feature.Endpoint).Returns(endpoint);
            context.Features.Set(endpointFeatureMock.Object);

            // when
            var invisibleMiddleware = new InvisibleApiMiddleware(
                next: requestDelegateMock.Object,
                visibilityHeader: randomInvisibleApiKey);

            await invisibleMiddleware.InvokeAsync(context);

            // then
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            endpointFeatureMock.Verify(feature => feature.Endpoint, Times.Once);
            requestDelegateMock.Verify(requestDelegate => requestDelegate(context), Times.Never);
        }

    }
}
