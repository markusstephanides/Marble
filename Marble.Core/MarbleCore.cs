using Marble.Core.Builder;

namespace Marble.Core
{
    public class MarbleCore
    {
        private static IAppHostBuilder appHostBuilder;

        public static IAppHostBuilder Builder => appHostBuilder ??= new DefaultAppHostBuilder();
    }
}