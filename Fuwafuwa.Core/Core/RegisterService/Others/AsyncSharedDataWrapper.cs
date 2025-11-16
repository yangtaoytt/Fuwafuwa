namespace Fuwafuwa.Core.Core.RegisterService.Others;

/// <summary>
///     The reference wrapper class.
///     Used to change the reference of TValue in a function.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class Ref<TValue> {
    public TValue Value;

    public Ref(TValue value) {
        Value = value;
    }
}

/// <summary>
///     The async shared data wrapper class.
///     Used to safely access shared data in async methods.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class AsyncSharedDataWrapper<TValue> {
    private readonly TValue _data;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public AsyncSharedDataWrapper(TValue data) {
        _data = data;
    }

    /// <summary>
    ///     The method to execute an async action with the shared data.
    /// </summary>
    /// <param name="asyncAction">Async action.</param>
    public async Task ExecuteAsync(Func<TValue, Task> asyncAction) {
        await _semaphore.WaitAsync();
        try {
            await asyncAction(_data);
        } finally {
            _semaphore.Release();
        }
    }

    /// <summary>
    ///     The method to execute an async function with the shared data and return a result.
    /// </summary>
    /// <param name="asyncFunc">Async function.</param>
    /// <typeparam name="TResult">The return type.</typeparam>
    /// <returns>Result.</returns>
    public async Task<TResult> ExecuteAsync<TResult>(Func<TValue, Task<TResult>> asyncFunc) {
        await _semaphore.WaitAsync();
        try {
            return await asyncFunc(_data);
        } finally {
            _semaphore.Release();
        }
    }
}