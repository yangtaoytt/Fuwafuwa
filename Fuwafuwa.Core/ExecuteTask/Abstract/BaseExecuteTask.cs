using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.ExecuteTask.Abstract;

public abstract class BaseExecuteTask : IComparable<BaseExecuteTask> {
    protected BaseExecuteTask(Priority priority, Type executorAttributeType) {
        Priority = priority;
        ExecutorAttributeType = executorAttributeType;
    }

    public Priority Priority { get; init; }

    public Type ExecutorAttributeType { get; init; }

    public int CompareTo(BaseExecuteTask? other) {
        if (ReferenceEquals(this, other)) {
            return 0;
        }

        if (other is null) {
            return 1;
        }

        return Priority.CompareTo(other.Priority);
    }
}