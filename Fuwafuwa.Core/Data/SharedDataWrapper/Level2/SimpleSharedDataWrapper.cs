using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWrapper.Level2;

public class SimpleSharedDataWrapper<T> : ISyncSharedDataWrapper<T, SimpleSharedDataWrapper<T>> {
    private readonly Reference<T> _data;
    private readonly Lock _lock = new();

    public SimpleSharedDataWrapper(T initialValue) {
        _data = new Reference<T>(initialValue);
    }

    public void Execute(Action<Reference<T>> action) {
        lock (_lock) {
            action(_data);
        }
    }

    public TResult Execute<TResult>(Func<Reference<T>, TResult> func) {
        lock (_lock) {
            return func(_data);
        }
    }

    public static SimpleSharedDataWrapper<T> CreateWrapper(T value) {
        return new SimpleSharedDataWrapper<T>(value);
    }
}