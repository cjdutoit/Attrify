// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.DeprecatedApi.Brokers.DateTimes;
using Attrify.DeprecatedApi.Services.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Tests.Unit.Controllers;
using Attrify.DeprecatedApi.Tests.Unit.Fakes;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Attrify.DeprecatedApi.Tests.Unit.Services.Foundations
{
    public partial class DeprecatedApiScannerServiceTests
    {
        private readonly ApplicationPartManager applicationPartManager;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly DeprecatedApiScannerService deprecatedApiScannerService;

        public DeprecatedApiScannerServiceTests()
        {
            // Configure necessary services
            var services = new ServiceCollection();
            services.AddControllersWithViews();
            services.AddRouting();
            var serviceProvider = services.BuildServiceProvider();

            dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            applicationPartManager = serviceProvider.GetRequiredService<ApplicationPartManager>();
            applicationPartManager.ApplicationParts.Add(new TestApplicationPart(typeof(TestController)));

            deprecatedApiScannerService =
                new DeprecatedApiScannerService(applicationPartManager, dateTimeBrokerMock.Object);
        }
    }
}
