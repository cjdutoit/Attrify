// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RESTFulSense.Clients.Extensions;
using RESTFulSense.Models;
using Xeptions;

namespace Attrify.DeprecatedApi.Tests.Unit.Controllers
{
    public partial class DeprecatedApiControllerTests
    {
        [Fact]
        public async Task ShouldReturnInternalServerErrorOnGetIfServerErrorOccurredAsync()
        {
            // given
            var someInnerException = new Xeption();
            string someMessage = GetRandomString();

            Xeption serverException = new DeprecatedApiScannerServiceException(
                message: someMessage,
                innerException: someInnerException);

            InternalServerErrorObjectResult expectedInternalServerErrorObjectResult =
                InternalServerError(serverException);

            var expectedActionResult =
                new ActionResult<List<DeprecatedApiInfo>>(expectedInternalServerErrorObjectResult);

            this.deprecatedApiScannerServiceMock.Setup(service =>
                service.GetDeprecatedApisAsync())
                    .ThrowsAsync(serverException);

            // when
            ActionResult<List<DeprecatedApiInfo>> actualActionResult =
                await this.deprecatedApiController.Get();

            // then
            actualActionResult.ShouldBeEquivalentTo(expectedActionResult);

            this.deprecatedApiScannerServiceMock.Verify(service =>
                service.GetDeprecatedApisAsync(),
                    Times.Once);

            this.deprecatedApiScannerServiceMock.VerifyNoOtherCalls();
        }
    }
}
