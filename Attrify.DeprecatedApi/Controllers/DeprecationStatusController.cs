// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Services.DeprecatedApiScanners;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace Attrify.DeprecatedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeprecationStatusController : RESTFulController
    {
        private readonly IDeprecatedApiScannerService deprecatedApiScannerService;

        public DeprecationStatusController(IDeprecatedApiScannerService deprecatedApiScannerService)
        {
            this.deprecatedApiScannerService = deprecatedApiScannerService;
        }

        [HttpGet]
        public async ValueTask<ActionResult<List<DeprecatedApiInfo>>> Get() =>
            throw new NotImplementedException();
    }
}
