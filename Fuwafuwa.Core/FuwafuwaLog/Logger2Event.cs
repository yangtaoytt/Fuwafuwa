using Fuwafuwa.Core.Log.LogEventArgs.Implement;

namespace Fuwafuwa.Core.Log;

public class Logger2Event {
    public Lock Lock { get; } = new();
    public event EventHandler<DebugLogEventArgs>? DebugLogGenerated;
    public event EventHandler<ErrorLogEventArgs>? ErrorLogGenerated;
    public event EventHandler<InfoLogEventArgs>? InfoLogGenerated;
    public event EventHandler<WarningLogEventArgs>? WarningLogGenerated;

    public void Error(object source, string message) {
        OnErrorLogGenerated(source, new ErrorLogEventArgs(message));
    }

    public void Info(object source, string message) {
        OnInfoLogGenerated(source, new InfoLogEventArgs(message));
    }

    public void Debug(object source, string message) {
        OnDebugLogGenerated(source, new DebugLogEventArgs(message));
    }
    // public void Warning(object source, string message) => OnWarningLogGenerated(new WarningLogEventArgs(source.GetType().Name + message));

    protected virtual void OnInfoLogGenerated(object sender, InfoLogEventArgs e) {
        lock (Lock) {
            InfoLogGenerated?.Invoke(sender, e);
        }
    }

    protected virtual void OnDebugLogGenerated(object sender, DebugLogEventArgs e) {
        lock (Lock) {
            DebugLogGenerated?.Invoke(sender, e);
        }
    }

    protected virtual void OnErrorLogGenerated(object sender, ErrorLogEventArgs e) {
        lock (Lock) {
            ErrorLogGenerated?.Invoke(sender, e);
        }
    }

    protected virtual void OnWarningLogGenerated(object sender, WarningLogEventArgs e) {
        lock (Lock) {
            WarningLogGenerated?.Invoke(sender, e);
        }
    }
}