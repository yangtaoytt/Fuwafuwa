using Fuwafuwa.Core.Log.LogEventArgs.Interface;

namespace Fuwafuwa.Core.Log.LogEventArgs.Implement;

public class DebugLogEventArgs : BaseLogEventArgs {
    public DebugLogEventArgs(string message) : base(message) {
        Level = LogLevel.Debug;
    }
}