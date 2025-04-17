namespace Fuwafuwa.Core.Log.LogEventArgs.Interface;

public class FuwafuwaLogEventArgs : EventArgs {
    public FuwafuwaLogEventArgs(string message) {
        Message = message;
        Timestamp = DateTime.UtcNow;
    }

    public string Message { get; init; }
    public DateTime Timestamp { get; init; }
}
