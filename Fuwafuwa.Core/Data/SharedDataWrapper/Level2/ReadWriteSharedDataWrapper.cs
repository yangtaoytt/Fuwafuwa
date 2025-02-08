using Fuwafuwa.Core.Data.SharedDataWrapper.Level1;
using Fuwafuwa.Core.Data.SharedDataWrapper.ReferenceBoxType;

namespace Fuwafuwa.Core.Data.SharedDataWrapper.Level2;

public class ReadWriteSharedDataWrapper<T> : ISyncSharedDataWrapper<T, ReadWriteSharedDataWrapper<T>> {
    private readonly Reference<T> _data;
    private readonly ReaderWriterLockSlim _rwLock = new();

    public ReadWriteSharedDataWrapper(T data) {
        _data = new Reference<T>(data);
    }

    public TResult Execute<TResult>(Func<Reference<T>, TResult> readAction) {
        _rwLock.EnterReadLock();
        try {
            return readAction(_data);
        } finally {
            _rwLock.ExitReadLock();
        }
    }

    public static ReadWriteSharedDataWrapper<T> CreateWrapper(T value) {
        return new ReadWriteSharedDataWrapper<T>(value);
    }

    public void Execute(Action<Reference<T>> writeAction) {
        _rwLock.EnterWriteLock();
        try {
            writeAction(_data);
        } finally {
            _rwLock.ExitWriteLock();
        }
    }
}