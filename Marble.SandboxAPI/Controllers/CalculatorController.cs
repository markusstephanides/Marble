using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Marble.SandboxAPI.Controllers
{
    [ApiController]
    [Route("calculator")]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> logger;

        public CalculatorController(ILogger<CalculatorController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("add")]
        public async Task<ActionResult<decimal>> GetSummationAsync([FromQuery] decimal a, [FromQuery] decimal b)
        {
            logger.LogInformation($"Got summation request {a}+{b}");
            return this.Ok(a + b);
        }
    }
}