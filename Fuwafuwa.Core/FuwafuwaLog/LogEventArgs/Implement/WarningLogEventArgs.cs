using Fuwafuwa.Core.Log.LogEventArgs.Interface;

namespace Fuwafuwa.Core.Log.LogEventArgs.Implement;

public class WarningLogEventArgs : BaseLogEventArgs {
    public WarningLogEventArgs(string message) : base(message) {
        Level = LogLevel.Warning;
    }
}