// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions;
using FluentAssertions;
using Moq;

namespace Attrify.DeprecatedApi.Tests.Unit.Services.Foundations
{
    public partial class DeprecatedApiScannerServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnGetDeprecatedApisIfServiceErrorOccurredAndLogItAsync()
        {
            // given
            var serviceException = new Exception();

            var failedServiceDeprecatedApiScannerExceptions =
                new FailedServiceDeprecatedApiScannerExceptions(
                    message: "Failed deprecated API scanner service error occurred, contact support.",
                    innerException: serviceException);

            var expectedDeprecatedApiScannerServiceException =
                new DeprecatedApiScannerServiceException(
                    message: "Deprecated API scanner service error occurred, contact support.",
                    innerException: failedServiceDeprecatedApiScannerExceptions);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<List<DeprecatedApiInfo>> getDeprecatedApisTask =
                this.deprecatedApiScannerService.GetDeprecatedApisAsync();

            DeprecatedApiScannerServiceException actualDeprecatedApiScannerServiceException =
                await Assert.ThrowsAsync<DeprecatedApiScannerServiceException>(
                    testCode: getDeprecatedApisTask.AsTask);

            // then
            actualDeprecatedApiScannerServiceException.Should().BeEquivalentTo(
                expectedDeprecatedApiScannerServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffsetAsync(),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
