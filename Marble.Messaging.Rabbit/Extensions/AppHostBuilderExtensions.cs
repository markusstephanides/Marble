using Marble.Core.Abstractions;
using Marble.Messaging.Extensions;
using Marble.Messaging.Rabbit.Configuration;

namespace Marble.Messaging.Rabbit.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithRabbitMessaging(this IAppHostBuilder builder,
            string configurationSection = "Messaging", RabbitConfiguration defaultConfiguration = null)
        {
            return builder.WithMessaging<RabbitMessagingAdapter, RabbitConfiguration>(configurationSection,
                defaultConfiguration);
        }
    }
}