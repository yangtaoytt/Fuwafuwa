using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWrapper.Level1;

public interface ISyncSharedDataWrapper<T, TWrapper> : ISharedDataWrapper 
    where TWrapper : ISyncSharedDataWrapper<T, TWrapper> {
    public void Execute(Action<Reference<T>> action);

    public TResult Execute<TResult>(Func<Reference<T>, TResult> func);
    
    public static abstract TWrapper CreateWrapper(T value);
}