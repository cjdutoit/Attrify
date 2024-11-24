// -----------------------------------------------------
// Copyright (c)  Christo du Toit - All rights reserved.
// -----------------------------------------------------

using System.Threading.Tasks;
using Attrify.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Attrify.WebApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvisibleApiController : ControllerBase
    {
        [HttpGet("visible")]
        public async ValueTask<ActionResult> VisibleAsync()
        {
            return Ok("This is a normal API.");
        }

        [HttpGet("invisible")]
        [InvisibleApi]
        public async ValueTask<ActionResult> InvisibleAsync()
        {
            return Ok($"This is an invisible API.");
        }
    }
}
