using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.ExecuteTask;

namespace Fuwafuwa.Core.Data.Implements;

public class TaskAgentData : IData {
    public TaskAgentData(ExecuteTaskSet executeTaskSet) {
        ExecuteTaskSet = executeTaskSet;
    }

    public ExecuteTaskSet ExecuteTaskSet { get; init; }
}