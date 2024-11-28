// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;

namespace Attrify.DeprecatedApi.Services.DeprecatedApiScanners
{
    internal interface IDeprecatedApiScannerService
    {
        ValueTask<List<DeprecatedApiInfo>> GetDeprecatedApisAsync();
    }
}
