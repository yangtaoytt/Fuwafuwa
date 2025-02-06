using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWrapper.Level1;

public interface IAsyncSharedDataWrapper<T,TWrapper> : ISharedDataWrapper
    where TWrapper : IAsyncSharedDataWrapper<T,TWrapper> {
    public Task ExecuteAsync(Func<Reference<T>, Task> asyncAction);

    public Task<TResult> ExecuteAsync<TResult>(Func<Reference<T>, Task<TResult>> asyncFunc);
    
    public static abstract TWrapper CreateWrapper(T value);
}