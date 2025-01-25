using Fuwafuwa.Core.Data.Interface;
using Fuwafuwa.Core.ExecuteTask.Abstract;

namespace Fuwafuwa.Core.Data.Implements;

public class ExecuteTaskData : IData {
    public ExecuteTaskData(BaseExecuteTask executeTask) {
        ExecuteTask = executeTask;
    }

    public BaseExecuteTask ExecuteTask { get; init; }
}