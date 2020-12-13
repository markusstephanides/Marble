using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Marble.Sandbox.Contracts;
using Marble.Sandbox.Contracts.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Marble.SandboxAPI.Controllers
{
    [ApiController]
    [Route("webshop")]
    public class WebshopController : ControllerBase
    {
        private readonly ILogger<WebshopController> logger;
        private readonly IWalletService walletService;
        private readonly IBasketService basketService;

        public WebshopController(
            ILogger<WebshopController> logger,
            IWalletService walletService,
            IBasketService basketService
        )
        {
            this.logger = logger;
            this.walletService = walletService;
            this.basketService = basketService;
        }

        [HttpPost("updateBasket")]
        public async Task<ActionResult> UpdateBasket(List<BasketItem> basketItems)
        {
            try
            {
                await this.basketService.UpdateBasket(basketItems).ConfigureAwait(false);
                return this.Ok();
            }
            catch (Exception e)
            {
                this.logger.LogError(e, $"Failed to send {nameof(UpdateBasket)} request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("checkout")]
        public async Task<ActionResult> Checkout()
        {
            try
            {
                var result = await this.basketService.Checkout().ConfigureAwait(false);
                return this.Ok(new {result});
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Failed to send " + nameof(Checkout) + " request");
                return this.StatusCode(500, new {message = e.Message});
            }
        }

        [HttpGet("userBalance")]
        public async Task<ActionResult> UserBalanceStream()
        {
            // TODO Stream Support for API
            return this.Ok();
        }

        public async Task WhyTheFuckUNotSupportStreamsDotNetCoreWebApi()
        {
            const string filePath = @"C:\Users\mike\Downloads\dotnet-sdk-3.1.201-win-x64.exe";
            this.Response.StatusCode = 200;
            this.Response.Headers.Add(HeaderNames.ContentDisposition,
                $"attachment; filename=\"{Path.GetFileName(filePath)}\"");
            this.Response.Headers.Add(HeaderNames.ContentType, "application/octet-stream");
            var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var outputStream = this.Response.Body;
            const int bufferSize = 1 << 10;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = await inputStream.ReadAsync(buffer, 0, bufferSize);
                if (bytesRead == 0) break;
                await outputStream.WriteAsync(buffer, 0, bytesRead);
            }

            await outputStream.FlushAsync();
        }
    }
}
