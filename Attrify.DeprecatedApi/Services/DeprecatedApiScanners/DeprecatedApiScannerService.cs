// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Brokers.DateTimes;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Attrify.DeprecatedApi.Services.DeprecatedApiScanners
{
    internal class DeprecatedApiScannerService : IDeprecatedApiScannerService
    {
        private readonly ApplicationPartManager partManager;
        private readonly IDateTimeBroker dateTimeBroker;

        public DeprecatedApiScannerService(ApplicationPartManager partManager, IDateTimeBroker dateTimeBroker)
        {
            this.partManager = partManager;
            this.dateTimeBroker = dateTimeBroker;
        }

        public async ValueTask<List<DeprecatedApiInfo>> GetDeprecatedApisAsync() =>
            throw new NotImplementedException();
    }
}
