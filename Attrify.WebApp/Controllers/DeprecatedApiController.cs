// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Threading.Tasks;
using Attrify.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Attrify.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeprecatedApiController : ControllerBase
    {
        [HttpGet("active")]
        public async ValueTask<ActionResult> ActiveAsync()
        {
            return Ok("This is an active API that is NOT configured for deprecation.");
        }

        [HttpGet("pending")]
        [DeprecatedApi(
            Sunset = "2084-11-22",
            Warning =
                "This API will sunset on 2084-11-22. " +
                "The link should provide details about alternatives, or migration steps.",
            Link = "https://api.example.com/deprecation-info")]
        public async ValueTask<ActionResult> PendingDeprecationAsync()
        {
            return Ok(
                "This is API has been flagged for deprecation. " +
                "This would be a normal api result, but you will see the deprecation headers");
        }

        [HttpGet("deprecated")]
        [DeprecatedApi(
            Sunset = "2022-11-22",
            Warning =
                $"This API will sunset on 2022-11-22. " +
                $"The link should provide details about alternatives, or migration steps.",
            Link = "https://api.example.com/deprecation-info")]
        public async ValueTask<ActionResult> DeprecatedAsync()
        {
            return Ok(
                "You will not see this, it will be replaced by the " +
                "410 Gone status code as this api has been deprecated.");
        }
    }
}
