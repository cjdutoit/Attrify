// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.DeprecatedApi.Controllers;
using Attrify.DeprecatedApi.Services.DeprecatedApiScanners;
using Moq;
using RESTFulSense.Controllers;
using Tynamix.ObjectFiller;

namespace Attrify.DeprecatedApi.Tests.Unit.Controllers
{
    public partial class DeprecatedApiControllerTests : RESTFulController
    {
        private readonly Mock<IDeprecatedApiScannerService> deprecatedApiScannerServiceMock;
        private readonly DeprecationStatusController deprecatedApiController;

        public DeprecatedApiControllerTests()
        {
            deprecatedApiScannerServiceMock = new Mock<IDeprecatedApiScannerService>();
            deprecatedApiController = new DeprecationStatusController(deprecatedApiScannerServiceMock.Object);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();
    }
}