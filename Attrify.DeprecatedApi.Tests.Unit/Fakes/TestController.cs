// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using Attrify.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Attrify.DeprecatedApi.Tests.Unit.Fakes
{
    // Test Controller for mocking purposes
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        [HttpGet("deprecated")]
        [DeprecatedApi(
            Sunset = "2022-11-22",
            Warning = "This API is deprecated.",
            Link = "https://api.example.com/deprecation-info")]
        public IActionResult DeprecatedAction()
        {
            return Ok();
        }

        [HttpGet("active")]
        public IActionResult ActiveAction()
        {
            return Ok();
        }
    }
}
