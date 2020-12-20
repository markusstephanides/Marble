using System;
using System.Threading;
using Marble.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marble.Core.Models
{
    public class AppHost
    {
        private readonly AppLifetime appLifetime;
        private readonly IEntryService? entryService;
        private readonly bool externallyHosted;
        private readonly DateTime initialCreationTime;
        private readonly ILogger<AppHost> logger;
        private readonly CancellationToken? providedCancellationToken;
        private readonly IServiceProvider serviceProvider;

        public AppHost(AppHostBuildingModel buildingModel)
        {
            this.serviceProvider = buildingModel.ServiceProvider!;
            this.appLifetime = buildingModel.AppLifetime;
            this.initialCreationTime = buildingModel.CreationTime;
            this.providedCancellationToken = buildingModel.ProvidedCancellationToken;
            this.externallyHosted = buildingModel.ShouldBeHostedExternally;

            this.entryService = this.serviceProvider.GetService<IEntryService>();
            this.logger = this.serviceProvider.GetService<ILogger<AppHost>>()!;
        }

        public void Run()
        {
            var elapsedTimeMs = (int) (DateTime.Now - this.initialCreationTime).TotalMilliseconds;

            this.logger.LogInformation("Startup completed in {elapsedTimeMs} ms", elapsedTimeMs);
            this.appLifetime.OnAppStarted.Invoke(this.serviceProvider);

            if (this.externallyHosted)
            {
                this.providedCancellationToken?.Register(() =>
                    this.appLifetime.OnAppStopping.Invoke(StopReason.ExternalCancellation));

                return;
            }

            this.entryService?.OnAppStarted(this.serviceProvider);

            var cancellationTokenSource = new CancellationTokenSource();
            var stopReason = StopReason.Unknown;

            Console.CancelKeyPress += (sender, args) =>
            {
                stopReason = StopReason.ConsoleCancellation;
                cancellationTokenSource.Cancel();
            };

            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Console.WriteLine("Shutdown requested");
                stopReason = stopReason == StopReason.Unknown ? StopReason.ProcessExit : stopReason;
                cancellationTokenSource.Cancel();
            };

            cancellationTokenSource.Token.WaitHandle.WaitOne();

            this.entryService?.OnAppStopping();
            this.appLifetime.OnAppStopping.Invoke(stopReason);
            ((IDisposable) this.serviceProvider).Dispose();
        }
    }
}