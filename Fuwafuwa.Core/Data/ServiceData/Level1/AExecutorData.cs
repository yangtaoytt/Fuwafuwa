using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Subjects;

namespace Fuwafuwa.Core.Data.ServiceData.Level1;

public abstract class AExecutorData : IServiceData, IComparable<AExecutorData> {
    protected AExecutorData(Priority priority, Type executorAttribute) {
        Priority = priority;
        ExecutorAttribute = executorAttribute;
    }

    public Priority Priority { get; init; }

    public Type ExecutorAttribute { get; init; }

    public int CompareTo(AExecutorData? other) {
        if (ReferenceEquals(this, other)) {
            return 0;
        }

        if (other is null) {
            return 1;
        }

        if (Priority != other.Priority) {
            return Priority.CompareTo(other.Priority);
        }
        
        return this.GetHashCode().CompareTo(other.GetHashCode());
    }
}