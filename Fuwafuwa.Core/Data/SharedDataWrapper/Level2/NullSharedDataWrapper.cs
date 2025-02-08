using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWrapper.Level2;

public class NullSharedDataWrapper<T> : ISyncSharedDataWrapper<T, NullSharedDataWrapper<T>> {
    private readonly Reference<T> _data;

    public NullSharedDataWrapper(T initialValue) {
        _data = new Reference<T>(initialValue);
    }

    public void Execute(Action<Reference<T>> action) {
        action(_data);
    }

    public TResult Execute<TResult>(Func<Reference<T>, TResult> func) {
        return func(_data);
    }

    public static NullSharedDataWrapper<T> CreateWrapper(T value) {
        return new NullSharedDataWrapper<T>(value);
    }
}