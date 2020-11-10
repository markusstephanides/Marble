using System;
using Marble.Core.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Marble.Utilities.Extensions
{
    public static class AppHostBuilderExtensions
    {
        public static IAppHostBuilder AddSingleton<TService>(this IAppHostBuilder builder)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddSingleton<TService>());
        }

        public static IAppHostBuilder AddSingleton<TService>(this IAppHostBuilder builder,
            TService instance)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddSingleton(instance));
        }

        public static IAppHostBuilder AddSingleton<TService>(this IAppHostBuilder builder,
            Func<TService, TService> factory)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddSingleton(factory));
        }

        public static IAppHostBuilder AddSingleton<TService>(this IAppHostBuilder builder,
            Func<IServiceProvider, TService> factory)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddSingleton(factory));
        }

        public static IAppHostBuilder AddTransient<TService>(this IAppHostBuilder builder)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddTransient<TService>());
        }
        
        public static IAppHostBuilder AddTransient<TAbstraction, TImplementation>(this IAppHostBuilder builder)
            where TAbstraction : class
            where TImplementation : class, TAbstraction
        {
            return builder.ConfigureServices(collection => collection.AddTransient<TAbstraction, TImplementation>());
        }

        public static IAppHostBuilder AddTransient<TService>(this IAppHostBuilder builder,
            Func<IServiceProvider, TService> factory)
            where TService : class
        {
            return builder.ConfigureServices(collection => collection.AddTransient(factory));
        }
    }
}