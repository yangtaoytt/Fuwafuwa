using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

public enum ServiceStrategyState {
    NotStarted,
    Running,
    Paused,
    ShutDown
}

/// <summary>
///     The abstract class for thread-safe service strategies.
///     Handle the _belongService's lifecycle and data processing.
///     Contains a ReaderWriterLockSlim to ensure thread safety.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public abstract class AThreadSafeServiceStrategy<TService> : IServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private readonly ReaderWriterLockSlim _lock = new();
    private AStrategyService<TService>? _belongService;
    private ServiceStrategyState _state = ServiceStrategyState.NotStarted;

    /// <summary>
    ///     Start the strategy with the belong service.
    ///     Do any initialization here.
    ///     **This function trys to get a write lock.**
    /// </summary>
    /// <param name="belongService">Save the service with this parameter.</param>
    public void Start(TService belongService) {
        _lock.EnterUpgradeableReadLock();
        try {
            if (_state != ServiceStrategyState.NotStarted) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.NotStarted,
                    "Service Strategy has already started.");
            }

            _belongService = belongService;
            ResumeInternal();
        } catch (Exception) {
            _lock.ExitUpgradeableReadLock();
            throw;
        }

        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Running;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }

    /// <summary>
    ///     Resume accepting new tasks.
    ///     **This function trys to get a write lock.**
    /// </summary>
    public void Resume() {
        _lock.EnterUpgradeableReadLock();
        try {
            if (_state != ServiceStrategyState.Paused) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Paused,
                    "Service Strategy is not paused.");
            }

            ResumeInternal();
        } catch (Exception) {
            _lock.ExitUpgradeableReadLock();
            throw;
        }

        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Running;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }

    /// <summary>
    ///     Shut down the threads.
    ///     **This function trys to get a write lock.**
    /// </summary>
    public void ShutDown() {
        _lock.EnterUpgradeableReadLock();
        try {
            if (_state != ServiceStrategyState.Running && _state != ServiceStrategyState.Paused) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                    ServiceStrategyState.Paused, "Service Strategy can not be shutdown.");
            }

            if (_state == ServiceStrategyState.Running) {
                WaitForCompletionInternal();
                _state = ServiceStrategyState.Paused;
            }

            Final();
        } catch (Exception) {
            _lock.ExitUpgradeableReadLock();
            throw;
        }

        _lock.EnterWriteLock();
        _state = ServiceStrategyState.ShutDown;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }

    /// <summary>
    ///     Stop accepting new tasks and wait for all running tasks to complete.
    ///     **This function trys to get a write lock.**
    /// </summary>
    public void WaitForCompletion() {
        _lock.EnterUpgradeableReadLock();
        try {
            if (_state != ServiceStrategyState.Running) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                    "Service Strategy is not running.");
            }

            WaitForCompletionInternal();
        } catch (Exception) {
            _lock.ExitUpgradeableReadLock();
            throw;
        }

        _lock.EnterWriteLock();
        _state = ServiceStrategyState.Paused;
        _lock.ExitWriteLock();
        _lock.ExitUpgradeableReadLock();
    }

    /// <summary>
    ///     Define What to do when receiving service data.
    ///     **This function trys to get a read lock.**
    /// </summary>
    /// <param name="serviceData">The coming data.</param>
    public void Receive(IServiceData<TService, object> serviceData) {
        _lock.EnterReadLock();
        try {
            if (_state != ServiceStrategyState.Running) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                    "Service Strategy is not running.");
            }

            ReceiveInternal(serviceData);
        } catch (Exception) {
            _lock.ExitReadLock();
            throw;
        }

        _lock.ExitReadLock();
    }

    /// <summary>
    ///     Define What to do when receiving service data.
    ///     Will not be called when the WaitForCompletion is called.
    ///     will be called again after Resume is called.
    ///     Can be called multiple times after the strategy has start.
    ///     **Should not contain the function that trys to get a write lock.**
    /// </summary>
    /// <param name="serviceData"></param>
    protected abstract void ReceiveInternal(IServiceData<TService, object> serviceData);

    /// <summary>
    ///     Stop accepting new tasks and wait for all running tasks to complete.
    ///     Should contain the logic to wait for all tasks to complete
    ///     and the logic to reset all the necessary states.
    ///     After this method is called,the ReceiveInternal will not be called until ResumeInternal is called,
    ///     so the strategy should still be able to resume accepting new tasks.
    ///     **Should not contain the function that trys to get a write lock.**
    /// </summary>
    protected abstract void WaitForCompletionInternal();

    /// <summary>
    ///     Resume accepting new tasks.
    ///     Should contain the logic to set up necessary states to accept new tasks.
    ///     **Should not contain the function that trys to get a write lock.**
    /// </summary>
    protected abstract void ResumeInternal();


    /// <summary>
    ///     Trigger finalization work for the _belongService.
    /// </summary>
    private void Final() {
        _lock.EnterReadLock();
        try {
            if (_state != ServiceStrategyState.Paused) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Paused,
                    "Service Strategy is not paused.");
            }

            _belongService!.Final();
        } catch (Exception) {
            _lock.ExitReadLock();
            throw;
        }

        _lock.ExitReadLock();
    }

    /// <summary>
    ///     Trigger the _belongService to work on the given data.
    ///     **This function trys to get a read lock.**
    /// </summary>
    /// <param name="data">Coming Data.</param>
    protected void WorkOnData(IServiceData<TService, object> data) {
        _lock.EnterReadLock();
        try {
            if (_state != ServiceStrategyState.Running) {
                throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                    "Service Strategy is not running.");
            }

            _belongService!.WorkOnData(data);
        } catch (Exception) {
            _lock.ExitReadLock();
            throw;
        }

        _lock.ExitReadLock();
    }
}