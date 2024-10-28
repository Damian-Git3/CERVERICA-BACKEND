namespace CERVERICA.Formatters
{
    public class CustomConsoleFormatter : ILogger
    {
        private readonly Dictionary<LogLevel, ConsoleColor> _colors = new()
        {
            [LogLevel.Debug] = ConsoleColor.DarkGray,
            [LogLevel.Information] = ConsoleColor.Green,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Critical] = ConsoleColor.DarkRed,
        };


        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => default!;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (_colors.ContainsKey(logLevel))
            {
                var originalColor = Console.ForegroundColor;
                Console.ForegroundColor = _colors[logLevel];
                Console.Write($"[{logLevel}] ");
                Console.ForegroundColor = originalColor;
                Console.WriteLine($"{formatter(state, exception)}");
            }
        }
    }
}
