using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Marble.Core.Logging
{
    public class DefaultConfigurations
    {
        public static readonly LoggingConfiguration ConsoleTargetConfiguration = new LoggingConfiguration();

        static DefaultConfigurations()
        {
            var consoleTarget = new ColoredConsoleTarget("Console")
            {
                Layout = new SimpleLayout(
                    "[${time}] ${level:uppercase=true}: ${message} ${exception} ${all-event-properties}")
            };

            foreach (var loggingLevel in LogLevel.AllLoggingLevels)
            {
                var highlightRule = new ConsoleWordHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression($"level == LogLevel.{loggingLevel.Name}"),
                    ForegroundColor = LogLevelToColor(loggingLevel),
                    Text = $"{loggingLevel.Name.ToUpper()}",
                    WholeWords = true
                };

                consoleTarget.WordHighlightingRules.Add(highlightRule);
            }

            var asyncWrapper = new AsyncTargetWrapper(consoleTarget);

            ConsoleTargetConfiguration.AddRule(LogLevel.Debug, LogLevel.Fatal, asyncWrapper);
        }

        private static ConsoleOutputColor LogLevelToColor(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
            {
                return ConsoleOutputColor.Green;
            }

            if (logLevel == LogLevel.Info)
            {
                return ConsoleOutputColor.Cyan;
            }

            if (logLevel == LogLevel.Warn)
            {
                return ConsoleOutputColor.Yellow;
            }

            if (logLevel == LogLevel.Trace)
            {
                return ConsoleOutputColor.Magenta;
            }

            if (logLevel == LogLevel.Error)
            {
                return ConsoleOutputColor.Red;
            }

            if (logLevel == LogLevel.Fatal)
            {
                return ConsoleOutputColor.DarkRed;
            }

            return ConsoleOutputColor.NoChange;
        }
    }
}