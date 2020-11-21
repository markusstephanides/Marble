using System;
using System.Runtime.CompilerServices;
using Marble.Core.Serialization;
using Marble.Messaging.Contracts.Configuration;

namespace Marble.Messaging.Utilities
{
    // TODO: Get rid of this, maybe we can use the AutoMapper lib for this
    public class ConfigurationMerger
    {
        private static MessagingConfiguration defaultConfiguration = new MessagingConfiguration
        {
            SerializationAdapterType = typeof(DefaultJsonSerializationAdapter),
            DefaultTimeoutInSeconds = 5
        };

        public static void Merge<TConfiguration>(TConfiguration originalConfiguration,
            TConfiguration codeConfiguration)
            where TConfiguration : MessagingConfiguration
        {
            var genericDefaultConfig = MapToGenericConfig<TConfiguration>(defaultConfiguration);
            if (originalConfiguration == null && codeConfiguration != null)
            {
                MergeWith(codeConfiguration,genericDefaultConfig);
                return;
            }
            if (codeConfiguration == null)
            {
                MergeWith(originalConfiguration,genericDefaultConfig);
                return;
            }

            // Merge default with code config (code > default)
            MergeWith(codeConfiguration, genericDefaultConfig);
            // Merge code config with file config (file > code)
            MergeWith(originalConfiguration, codeConfiguration);
        }

        private static void MergeWith<TConfiguration>(TConfiguration primary, TConfiguration secondary)
            where TConfiguration : MessagingConfiguration
        {
            foreach (var pi in typeof(TConfiguration).GetProperties())
            {
                var primaryValue = pi.GetGetMethod().Invoke(primary, null);
                var secondaryValue = pi.GetGetMethod().Invoke(secondary, null);

                if (primaryValue == null || pi.PropertyType.IsValueType &&
                    primaryValue.Equals(Activator.CreateInstance(pi.PropertyType)))
                {
                    pi.GetSetMethod().Invoke(primary, new[] {secondaryValue});
                }
                else
                {
                    pi.GetSetMethod().Invoke(primary, new[] {primaryValue});
                }
            }
        }

        private static TConfiguration MapToGenericConfig<TConfiguration>(MessagingConfiguration messagingConfiguration)
        {
            var resultingConfig = Activator.CreateInstance<TConfiguration>();

            foreach (var pi in typeof(TConfiguration).GetProperties())
            {
                var value = pi.GetGetMethod().Invoke(messagingConfiguration, null);
                pi.GetSetMethod().Invoke(resultingConfig, new[] {value});
            }

            return resultingConfig;
        }
    }
}