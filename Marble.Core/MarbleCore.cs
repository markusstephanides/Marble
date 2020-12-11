using Marble.Core.Builder;
using Marble.Core.Builder.Abstractions;

namespace Marble.Core
{
    public class MarbleCore
    {
        private static IAppHostBuilder? appHostBuilder;

        public static IAppHostBuilder Builder => appHostBuilder ??= new DefaultAppHostBuilder();
    }
}