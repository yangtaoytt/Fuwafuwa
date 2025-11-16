using System.IO.IsolatedStorage;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

enum ServiceStrategyState {
    NotStarted,
    Running,
    Paused,
    ShutDown
}
/// <summary>
///     Handle the _belongService's lifecycle and data processing.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public abstract class AServiceStrategy<TService> : IServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private AStrategyService<TService>? _belongService;
    private ServiceStrategyState _state = ServiceStrategyState.NotStarted;
    private readonly ReaderWriterLockSlim  _lock = new();

    public void Start(TService belongService) {
        _lock.EnterUpgradeableReadLock();
        if (_state != ServiceStrategyState.NotStarted) {
            throw new Exception("Service Strategy has already started.");
        }
        _belongService = belongService;
        ResumeInternal();
        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Running;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }
    public void Resume() {
        _lock.EnterUpgradeableReadLock();
        if (_state != ServiceStrategyState.Paused) {
            throw new Exception("Service Strategy is not paused.");
        }
        ResumeInternal();
        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Running;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }
    public void ShutDown() {
        _lock.EnterUpgradeableReadLock();
        if (_state != ServiceStrategyState.Running && _state != ServiceStrategyState.Paused) {
            throw new Exception("Service Strategy can not be shutdown.");
        }
        if (_state == ServiceStrategyState.Running) {
            WaitForCompletionInternal();
        }
        Final();
        _lock.EnterWriteLock();
        _state = ServiceStrategyState.ShutDown;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }
    public void WaitForCompletion() {
        _lock.EnterUpgradeableReadLock();
        if (_state != ServiceStrategyState.Running) {
            throw new Exception("Service Strategy is not running.");
        }
        WaitForCompletionInternal();
        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Paused;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }
    
    public void Receive(IServiceData<TService, object> serviceData) {
        _lock.EnterReadLock();
        if (_state != ServiceStrategyState.Running) {
            throw new Exception("Service Strategy is not running.");
        }
        ReceiveInternal(serviceData);
        _lock.ExitReadLock();
    }

    protected abstract void ReceiveInternal(IServiceData<TService, object> serviceData);

    protected abstract void WaitForCompletionInternal();
    
    protected abstract void ResumeInternal();
    

    /// <summary>
    ///     Trigger finalization work for the _belongService.
    /// </summary>
    private void Final() {
        _lock.EnterReadLock();
        if (_state != ServiceStrategyState.Paused) {
            throw new Exception("Service Strategy is not paused.");
        }
        _belongService!.Final();
        _lock.ExitReadLock();
    }

    /// <summary>
    ///     Trigger the _belongService to work on the given data.
    /// </summary>
    /// <param name="data">Coming Data.</param>
    protected void WorkOnData(IServiceData<TService, object> data) {
        _lock.EnterReadLock();
        if (_state!!= ServiceStrategyState.Running) {
            throw new Exception("Service Strategy is not running.");
        }
        _belongService!.WorkOnData(data);
        _lock.ExitReadLock();
    }
}