using Marble.Core.Builder;

namespace Marble.Messaging.Rabbit.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder WithRabbitMessaging(this IAppHostBuilder builder)
        {
            return builder.WithMessaging<RabbitMessagingClient>();
        }
    }
}