using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWapper.Implement;

public class SimpleSharedDataWrapper<T> : ISyncSharedDataWrapper<T,SimpleSharedDataWrapper<T>>
{
    private readonly Lock _lock = new Lock();
    private readonly Reference<T> _data;

    public SimpleSharedDataWrapper(T initialValue)
    {
        _data = new Reference<T>(initialValue);
    }
    
    public void Execute(Action<Reference<T>> action)
    {
        lock (_lock)
        {
            action(_data);
        }
    }
    
    public TResult Execute<TResult>(Func<Reference<T>, TResult> func)
    {
        lock (_lock)
        {
            return func(_data);
        }
    }

    public static SimpleSharedDataWrapper<T> CreateWrapper(T value) {
        return new SimpleSharedDataWrapper<T>(value);
    }
}