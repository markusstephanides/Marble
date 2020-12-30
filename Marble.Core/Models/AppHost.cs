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

        public AppHost(AppHostBuildingModel buildingModel)
        {
            this.ServiceProvider = buildingModel.ServiceProvider!;
            this.appLifetime = buildingModel.AppLifetime;
            this.initialCreationTime = buildingModel.CreationTime;
            this.providedCancellationToken = buildingModel.ProvidedCancellationToken;
            this.externallyHosted = buildingModel.ShouldBeHostedExternally;

            this.entryService = this.ServiceProvider.GetService<IEntryService>();
            this.logger = this.ServiceProvider.GetService<ILogger<AppHost>>()!;
        }

        public IServiceProvider ServiceProvider { get; }

        public void Stop()
        {
            Environment.Exit(0);
        }

        internal void Run()
        {
            var elapsedTimeMs = (int) (DateTime.Now - this.initialCreationTime).TotalMilliseconds;

            this.appLifetime.OnAppStarted.Invoke(this.ServiceProvider);
            this.logger.LogInformation("Startup completed in {elapsedTimeMs} ms", elapsedTimeMs);

            if (this.externallyHosted)
            {
                this.providedCancellationToken?.Register(() =>
                    this.appLifetime.OnAppStopping.Invoke(StopReason.ExternalCancellation));

                return;
            }

            this.entryService?.OnAppStarted(this.ServiceProvider);

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

            new Thread(() =>
            {
                cancellationTokenSource.Token.WaitHandle.WaitOne();
                this.ShutdownActions(stopReason);
            }).Start();
        }

        private void ShutdownActions(StopReason stopReason)
        {
            this.entryService?.OnAppStopping();
            this.appLifetime.OnAppStopping.Invoke(stopReason);
            ((IDisposable) this.ServiceProvider).Dispose();
        }
    }
}