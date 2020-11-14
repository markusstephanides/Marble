using Marble.Core.Builder;
using Marble.Core.Builder.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Marble.Logging
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithLogging(this IAppHostBuilder builder,
            LogLevel minimumLevel = LogLevel.Information)
        {
            return builder.ConfigureServices(collection =>
            {
                collection.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(minimumLevel);
                    loggingBuilder.AddNLog(DefaultConfigurations.ConsoleTargetConfiguration);
                });
            });
        }
    }
}