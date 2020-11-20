using System;
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

        public static TConfiguration Merge<TConfiguration>(TConfiguration fileConfiguration,
            TConfiguration codeConfiguration)
            where TConfiguration : MessagingConfiguration
        {
            if (fileConfiguration == null && codeConfiguration != null) return codeConfiguration;
            if (fileConfiguration != null && codeConfiguration == null) return fileConfiguration;
            if (fileConfiguration == null && codeConfiguration == null) return null;
            
            // Merge default with code config (code > default)
            var resultingConfig = MergeWith(codeConfiguration,
                MapToGenericConfig<TConfiguration>(defaultConfiguration));
            return MergeWith(fileConfiguration, resultingConfig);
        }

        private static TConfiguration MergeWith<TConfiguration>(TConfiguration primary, TConfiguration secondary)
            where TConfiguration : MessagingConfiguration
        {
            var resultingConfig = Activator.CreateInstance<TConfiguration>();

            foreach (var pi in typeof(TConfiguration).GetProperties())
            {
                var primaryValue = pi.GetGetMethod().Invoke(primary, null);
                var secondaryValue = pi.GetGetMethod().Invoke(secondary, null);

                if (primaryValue == null || pi.PropertyType.IsValueType &&
                    primaryValue.Equals(Activator.CreateInstance(pi.PropertyType)))
                {
                    pi.GetSetMethod().Invoke(resultingConfig, new[] {secondaryValue});
                }
                else
                {
                    pi.GetSetMethod().Invoke(resultingConfig, new[] {primaryValue});
                }
            }

            return resultingConfig;
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