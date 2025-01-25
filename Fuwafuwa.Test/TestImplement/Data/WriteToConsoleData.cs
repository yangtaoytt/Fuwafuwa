using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Subjects;
using Fuwafuwa.Test.TestImplement.Attribute.Executor;

namespace Fuwafuwa.Test.TestImplement.Data;

public class WriteToConsoleData : AExecutorData {
    public WriteToConsoleData(Priority priority, string message) : base(priority, typeof(IWriteToConsole)) {
        Message = message;
    }

    public string Message { get; init; }
}