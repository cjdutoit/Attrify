// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;

namespace Attrify.DeprecatedApi.Tests.Unit.Controllers
{
    public partial class DeprecatedApiControllerTests
    {
        [Fact]
        public async Task ShouldReturnRecordsOnGetAsync()
        {
            // given
            var expectedDeprecatedApis = new List<DeprecatedApiInfo>
            {
                new DeprecatedApiInfo
                {
                    Controller = "Test",
                    Action = "DeprecatedAction",
                    HttpMethods = new List<string> { "GET" },
                    Route = "api/test/deprecated",
                    Sunset = "2022-11-22",
                    Warning = "This API is deprecated.",
                    Link = "https://api.example.com/deprecation-info",
                    IsDeprecated = true
                }
            };

            var expectedObjectResult =
                new OkObjectResult(expectedDeprecatedApis);

            var expectedActionResult =
                new ActionResult<List<DeprecatedApiInfo>>(expectedObjectResult);

            deprecatedApiScannerServiceMock.Setup(service =>
                service.GetDeprecatedApisAsync())
                    .ReturnsAsync(expectedDeprecatedApis);

            // when
            ActionResult<List<DeprecatedApiInfo>> actualActionResult = await deprecatedApiController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            deprecatedApiScannerServiceMock.Verify(service =>
                service.GetDeprecatedApisAsync(),
                    Times.Once);

            deprecatedApiScannerServiceMock.VerifyNoOtherCalls();
        }
    }
}
