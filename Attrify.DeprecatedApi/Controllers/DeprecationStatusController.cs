// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners;
using Attrify.DeprecatedApi.Models.Foundations.DeprecatedApiScanners.Exceptions;
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
        public async ValueTask<ActionResult<List<DeprecatedApiInfo>>> Get()
        {
            try
            {
                var deprecatedApis = await deprecatedApiScannerService.GetDeprecatedApisAsync();

                return Ok(deprecatedApis);
            }
            catch (DeprecatedApiScannerServiceException deprecatedApiScannerServiceException)
            {
                return InternalServerError(deprecatedApiScannerServiceException);
            }
        }
    }
}
