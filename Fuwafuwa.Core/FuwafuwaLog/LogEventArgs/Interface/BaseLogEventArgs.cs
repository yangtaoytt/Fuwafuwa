namespace Fuwafuwa.Core.Log.LogEventArgs.Interface;

public abstract class BaseLogEventArgs : EventArgs {
    protected BaseLogEventArgs(string message) {
        Message = message;
        Level = LogLevel.Error;
        Timestamp = DateTime.UtcNow; // 自动记录时间戳
    }

    public string Message { get; init; } // 日志内容
    public DateTime Timestamp { get; init; } // 日志时间戳
    public LogLevel Level { get; protected set; } // 日志级别（如 Info、Warning、Error）
}

public enum LogLevel {
    Debug,
    Info,
    Warning,
    Error
}