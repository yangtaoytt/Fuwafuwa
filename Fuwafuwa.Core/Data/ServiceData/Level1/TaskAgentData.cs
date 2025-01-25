using Fuwafuwa.Core.Data.ServiceData.Level0;

namespace Fuwafuwa.Core.Data.ServiceData.Level1;

public class TaskAgentData : IServiceData {
    public TaskAgentData(ExecuteDataSet.ExecuteDataSet executeTaskSet) {
        ExecuteTaskSet = executeTaskSet;
    }

    public ExecuteDataSet.ExecuteDataSet ExecuteTaskSet { get; init; }
}