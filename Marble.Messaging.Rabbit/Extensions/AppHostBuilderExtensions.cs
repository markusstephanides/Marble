using Marble.Core.Builder;
using Marble.Core.Builder.Abstractions;
using Marble.Messaging.Extensions;
using Marble.Messaging.Rabbit.Models;

namespace Marble.Messaging.Rabbit.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithRabbitMessaging(this IAppHostBuilder builder,
            string configurationSection = "Messaging", RabbitConfiguration defaultConfiguration = null)
        {
            return builder.WithMessaging<RabbitMessagingAdapter, RabbitConfiguration>(configurationSection, defaultConfiguration);
        }
    }
}