namespace Fuwafuwa.Core.Log;

public class Logger2Event {
    private static readonly Lazy<Logger2Event> LazyInstance = new(() => new Logger2Event());
    
    public static Logger2Event Instance => LazyInstance.Value;
    
    public event EventHandler<LogEventArgs.Interface.FuwafuwaLogEventArgs>? DebugLogGenerated;
    public event EventHandler<LogEventArgs.Interface.FuwafuwaLogEventArgs>? ErrorLogGenerated;
    public event EventHandler<LogEventArgs.Interface.FuwafuwaLogEventArgs>? InfoLogGenerated;
    public event EventHandler<LogEventArgs.Interface.FuwafuwaLogEventArgs>? WarningLogGenerated;

    public void Error(object source, string message) => ErrorLogGenerated?.Invoke(source, new LogEventArgs.Interface.FuwafuwaLogEventArgs(message));

    public void Info(object source, string message) => InfoLogGenerated?.Invoke(source, new LogEventArgs.Interface.FuwafuwaLogEventArgs(message));

    public void Debug(object source, string message) => DebugLogGenerated?.Invoke(source, new LogEventArgs.Interface.FuwafuwaLogEventArgs(message));

    public void Warning(object source, string message) => WarningLogGenerated?.Invoke(source, new LogEventArgs.Interface.FuwafuwaLogEventArgs(message));
}