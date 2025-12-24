using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace WebAppMultiTenant;

/// <summary>
/// Formatter for Serilog-like console logging.
/// </summary>
public class SerilogLikeConsoleFormatter : ConsoleFormatter
{
    public SerilogLikeConsoleFormatter() : base("serilog-like")
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        textWriter.WriteLine(
            $"[{DateTime.Now:HH:mm:ss} {logEntry.LogLevel.ToString()[..3].ToUpper()}] " +
            $"{logEntry.Category} {logEntry.Formatter(logEntry.State, logEntry.Exception)}"
        );
    }
}