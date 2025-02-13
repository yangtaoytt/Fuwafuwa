using Fuwafuwa.Core.Log.LogEventArgs.Interface;

namespace Fuwafuwa.Core.Log.LogEventArgs.Implement;

public class InfoLogEventArgs : BaseLogEventArgs {
    public InfoLogEventArgs(string message) : base(message) {
        Level = LogLevel.Info;
    }
}