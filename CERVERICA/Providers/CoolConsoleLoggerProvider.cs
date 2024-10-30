using CERVERICA.Formatters;
using System.Collections.Concurrent;

namespace CERVERICA.Providers
{
    public sealed class CoolConsoleLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, CustomConsoleFormatter> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public ILogger CreateLogger(string categoryName)
            => _loggers.GetOrAdd(categoryName, name => new CustomConsoleFormatter());

        public void Dispose()
            => _loggers.Clear();
    }
}
