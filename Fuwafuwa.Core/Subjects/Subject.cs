namespace Fuwafuwa.Core.Subjects;

public class Subject {
    private const long DifferentRange = 10000000;
    private static readonly Random Random = new();

    private Subject(long uniqueId) {
        UniqueId = uniqueId;
    }

    public long UniqueId { get; init; }

    public static Subject GetSubject() {
        var ticks = DateTime.Now.Ticks;
        var randomInt = Random.NextInt64(0, DifferentRange);
        var uniqueId = ticks - DifferentRange + randomInt;

        return new Subject(uniqueId);
    }
}