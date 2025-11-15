namespace Fuwafuwa.Core.Logger.LogEventArgs.Interface;

/// <summary>
///     The event arguments for fuwafuwa log events.
/// </summary>
public class FuwafuwaLogEventArgs : EventArgs {
    public FuwafuwaLogEventArgs(string message) {
        Message = message;
        Timestamp = DateTime.UtcNow;
    }

    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
}