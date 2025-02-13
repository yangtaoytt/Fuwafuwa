using Fuwafuwa.Core.Log.LogEventArgs.Interface;

namespace Fuwafuwa.Core.Log.LogEventArgs.Implement;

public class ErrorLogEventArgs : BaseLogEventArgs {
    public ErrorLogEventArgs(string message) : base(message) {
        Level = LogLevel.Error;
    }
}