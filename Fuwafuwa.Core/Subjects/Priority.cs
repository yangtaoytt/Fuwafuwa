namespace Fuwafuwa.Core.Subjects;

public enum PriorityStrategy {
    Unique,
    Share
}

public class Priority : IComparable<Priority> {
    public Priority(int value, PriorityStrategy strategy) {
        Value = value;
        Strategy = strategy;
    }

    public int Value { get; init; }

    public PriorityStrategy Strategy { get; init; }

    public int CompareTo(Priority? other) {
        if (ReferenceEquals(this, other)) {
            return 0;
        }

        if (other is null) {
            return 1;
        }

        var valueComparison = Value.CompareTo(other.Value);
        return valueComparison;
    }
}