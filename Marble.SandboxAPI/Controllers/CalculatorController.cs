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
        private readonly IMathServiceClient mathService;

        public CalculatorController(
            ILogger<CalculatorController> logger,
            IMathServiceClient mathService
        )
        {
            this.logger = logger;
            this.mathService = mathService;
        }

        [HttpGet("addReturnInt")]
        public async Task<ActionResult> AddReturnInt([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnInt(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnInt request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnIntThrowException")]
        public async Task<ActionResult> AddReturnIntThrowException([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnIntThrowException(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnIntThrowException request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnObject")]
        public async Task<ActionResult> AddReturnObject([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnObject(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnObject request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnTask")]
        public async Task<ActionResult> AddReturnTask([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                await this.mathService.AddReturnTask(a, b).ConfigureAwait(false);
                return this.Ok(new {status = "Sent!"});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnTask request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnVoid")]
        public async Task<ActionResult> AddReturnVoid([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                await this.mathService.AddReturnVoid(a, b).ConfigureAwait(false);
                return this.Ok(new {status = "Sent!"});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnVoid request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnTaskInt")]
        public async Task<ActionResult> AddReturnTaskInt([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnTaskInt(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnTaskInt request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("addReturnTaskObject")]
        public async Task<ActionResult> AddReturnTaskObject([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnTaskObject(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnTaskObject request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }


        [HttpGet("addReturnTaskObjectThrowException")]
        public async Task<ActionResult> AddReturnTaskObjectThrowException([FromQuery] int a, [FromQuery] int b)
        {
            try
            {
                var result = await this.mathService.AddReturnTaskObjectThrowException(a, b).ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send AddReturnTaskObjectThrowException request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("startMathStreamReturnInt")]
        public async Task<ActionResult> StartMathStreamReturnInt([FromQuery] int start)
        {
            try
            {
                this.mathService.StartMathStreamReturnInt(start).Subscribe(
                    value => { this.logger.LogInformation($"Received value {value} from StartMathStreamReturnInt"); },
                    error => { this.logger.LogError($"StartMathStreamReturnInt-Stream ERROR: {error}"); },
                    () => { this.logger.LogInformation("StartMathStreamReturnInt-Stream completed!"); });
                return this.Ok(new {status = "Sent!"});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send StartMathStreamReturnInt request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("startMathStreamReturnObject")]
        public async Task<ActionResult> StartMathStreamReturnObject([FromQuery] int start)
        {
            try
            {
                this.mathService.StartMathStreamReturnObject(start).Subscribe(
                    value =>
                    {
                        this.logger.LogInformation($"Received value {value.SomeInt} from StartMathStreamReturnObject");
                    }, error => { this.logger.LogError($"StartMathStreamReturnObject-Stream ERROR: {error}"); },
                    () => { this.logger.LogInformation("StartMathStreamReturnObject-Stream completed!"); });
                return this.Ok(new {status = "Sent!"});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send StartMathStreamReturnObject request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("startMathStreamReturnObjectThrowException")]
        public async Task<ActionResult> StartMathStreamReturnObjectThrowException([FromQuery] int start)
        {
            try
            {
                this.mathService.StartMathStreamReturnObjectThrowException(start).Subscribe(
                    value =>
                    {
                        this.logger.LogInformation(
                            $"Received value {value.SomeInt} from StartMathStreamReturnObjectThrowException");
                    },
                    error =>
                    {
                        this.logger.LogError($"StartMathStreamReturnObjectThrowException-Stream ERROR: {error}");
                    },
                    () =>
                    {
                        this.logger.LogInformation("StartMathStreamReturnObjectThrowException-Stream completed!");
                    });
                return this.Ok(new {status = "Sent!"});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send StartMathStreamReturnObjectThrowException request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }
    }
}