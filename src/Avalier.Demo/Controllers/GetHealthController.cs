using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Avalier.Sentry.Controllers
{
    [ApiController]
    [Route("health")]
    [Route("healthz")]
    [Route("api/health")]
    [Route("api/healthz")]
    public class GetHealthController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}