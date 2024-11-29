// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions;
using Xeptions;

namespace Attrify.DeprecatedApi.Services.DeprecatedApiScanners
{
    public partial class DeprecatedApiScannerService : IDeprecatedApiScannerService
    {
        private delegate ValueTask<List<DeprecatedApiInfo>> ReturningDeprecatedApiInfoListFunction();

        private async ValueTask<List<DeprecatedApiInfo>> TryCatch(
            ReturningDeprecatedApiInfoListFunction returningDeprecatedApiInfoListFunction)
        {
            try
            {
                return await returningDeprecatedApiInfoListFunction();
            }
            catch (Exception exception)
            {
                var failedServiceDeprecatedApiScannerExceptions =
                    new FailedServiceDeprecatedApiScannerExceptions(
                        message: "Failed deprecated API scanner service error occurred, contact support.",
                        innerException: exception);

                throw await CreateServiceExceptionAsync(failedServiceDeprecatedApiScannerExceptions);
            }
        }

        private async ValueTask<DeprecatedApiScannerServiceException> CreateServiceExceptionAsync(Xeption exception)
        {
            var deprecatedApiScannerServiceException = new DeprecatedApiScannerServiceException(
                message: "Deprecated API scanner service error occurred, contact support.",
                innerException: exception);

            return deprecatedApiScannerServiceException;
        }
    }
}
