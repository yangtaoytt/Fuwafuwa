using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWapper.Implement;

public class AsyncSharedDataWrapper<T> : IAsyncSharedDataWrapper<T, AsyncSharedDataWrapper<T>> {
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly Reference<T> _data;

    public AsyncSharedDataWrapper(T initialValue) {
        _data = new Reference<T>(initialValue);
    }

    public async Task ExecuteAsync(Func<Reference<T>, Task> asyncAction) {
        await _semaphore.WaitAsync();
        try {
            await asyncAction(_data);
        } finally {
            _semaphore.Release();
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<Reference<T>, Task<TResult>> asyncFunc) {
        await _semaphore.WaitAsync();
        try {
            return await asyncFunc(_data);
        } finally {
            _semaphore.Release();
        }
    }

    public static AsyncSharedDataWrapper<T> CreateWrapper(T value) {
        return new AsyncSharedDataWrapper<T>(value);
    }
}