using Marble.Core.Builder;
using Marble.Messaging.Rabbit.Models;

namespace Marble.Messaging.Rabbit.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithRabbitMessaging(this IAppHostBuilder builder,
            string configurationSection = "Messaging")
        {
            return builder.WithMessaging<RabbitMessagingClient, RabbitClientConfiguration>(configurationSection);
        }
    }
}