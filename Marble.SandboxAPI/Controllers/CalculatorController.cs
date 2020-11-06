using System;
using System.Threading.Tasks;
using Marble.Sandbox.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marble.SandboxAPI.Controllers
{
    [ApiController]
    [Route("calculator")]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> logger;
        private readonly IMathService mathService;

        public CalculatorController(
            ILogger<CalculatorController> logger,
            IMathService mathService
        )
        {
            this.logger = logger;
            this.mathService = mathService;
        }

        [HttpGet("add")]
        public async Task<ActionResult<int>> GetAdditionAsync([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await mathService.ComplexAdd(a, b).ConfigureAwait(false);
                return Ok(new {result});
            }
            catch (Exception e)
            {
                return StatusCode(500, new {message = e.Message});
            }
        }
    }
}