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
        public async Task<ActionResult<int>> GetSummationAsync([FromQuery] int a, [FromQuery] int b)
        {
            var result = await this.mathService.Add(a, b);
            logger.LogInformation($"Got summation request {a}+{b} = {result}");
            return this.Ok(result);
        }
    }
}
