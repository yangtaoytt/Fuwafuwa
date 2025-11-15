using Fuwafuwa.Core.Logger.LogEventArgs.Interface;

namespace Fuwafuwa.Core.Logger;

/// <summary>
///     The logger that uses events to notify log messages.
/// </summary>
public class Logger2Event {
    private static readonly Lazy<Logger2Event> LazyInstance = new(() => new Logger2Event());

    public static Logger2Event Instance => LazyInstance.Value;

    public event EventHandler<FuwafuwaLogEventArgs>? DebugLogGenerated;
    public event EventHandler<FuwafuwaLogEventArgs>? ErrorLogGenerated;
    public event EventHandler<FuwafuwaLogEventArgs>? InfoLogGenerated;
    public event EventHandler<FuwafuwaLogEventArgs>? WarningLogGenerated;

    /// <summary>
    ///     Publishes an error log event.
    /// </summary>
    /// <param name="source">The source of the log event.Will be passed as the sender in the event handler.</param>
    /// <param name="message">The error message.</param>
    public void Error(object source, string message) {
        ErrorLogGenerated?.Invoke(source, new FuwafuwaLogEventArgs(message));
    }

    /// <summary>
    ///     Publishes an info log event.
    /// </summary>
    /// <param name="source">The source of the log event.Will be passed as the sender in the event handler.</param>
    /// <param name="message">The info message.</param>
    public void Info(object source, string message) {
        InfoLogGenerated?.Invoke(source, new FuwafuwaLogEventArgs(message));
    }

    /// <summary>
    ///     Publishes a debug log event.
    /// </summary>
    /// <param name="source">The source of the log event.Will be passed as the sender in the event handler.</param>
    /// <param name="message">The debug message.</param>
    public void Debug(object source, string message) {
        DebugLogGenerated?.Invoke(source, new FuwafuwaLogEventArgs(message));
    }

    /// <summary>
    ///     Publishes a warning log event.
    /// </summary>
    /// <param name="source">The source of the log event.Will be passed as the sender in the event handler.</param>
    /// <param name="message">The warning message.</param>
    public void Warning(object source, string message) {
        WarningLogGenerated?.Invoke(source, new FuwafuwaLogEventArgs(message));
    }
}