using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;
using Fuwafuwa.Core.Core.Service.ServiceStrategy.ThreadSafeServiceStrategy;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy.SimpleServiceStrategy;

/// <summary>
///     The abstract class for single thread service strategies.
///     It provides basic state management for the strategy.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public abstract class ASimpleServiceStrategy<TService> : IServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private AStrategyService<TService>? _belongService;
    private ServiceStrategyState _state = ServiceStrategyState.NotStarted;

    public void Start(TService belongService) {
        if (_state != ServiceStrategyState.NotStarted) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.NotStarted,
                "Service Strategy has already started.");
        }

        _belongService = belongService;
        ResumeInternal();
        _state = ServiceStrategyState.Running;
    }

    public void Resume() {
        if (_state != ServiceStrategyState.Paused) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Paused,
                "Service Strategy is not paused.");
        }

        ResumeInternal();
        _state = ServiceStrategyState.Running;
    }

    public void ShutDown() {
        if (_state != ServiceStrategyState.Running && _state != ServiceStrategyState.Paused) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                ServiceStrategyState.Paused, "Service Strategy can not be shutdown.");
        }

        if (_state == ServiceStrategyState.Running) {
            WaitForCompletionInternal();
        }

        Final();
        _state = ServiceStrategyState.ShutDown;
    }

    public void WaitForCompletion() {
        if (_state != ServiceStrategyState.Running) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                "Service Strategy is not running.");
        }

        WaitForCompletionInternal();
        _state = ServiceStrategyState.Paused;
    }

    public void Receive(IServiceData<TService, object> serviceData) {
        if (_state != ServiceStrategyState.Running) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                "Service Strategy is not running.");
        }

        ReceiveInternal(serviceData);
    }

    /// <summary>
    ///     Define What to do when receiving service data.
    ///     Will not be called when the WaitForCompletion is called.
    ///     Will be called again after Resume is called.
    ///     Can be called multiple times after the strategy has start.
    /// </summary>
    /// <param name="serviceData">The coming data.</param>
    protected abstract void ReceiveInternal(IServiceData<TService, object> serviceData);

    /// <summary>
    ///     Stop accepting new tasks and wait for all running tasks to complete.
    ///     Should contain the logic to wait for all tasks to complete
    ///     and the logic to reset all the necessary states.
    ///     After this method is called,the ReceiveInternal will not be called until ResumeInternal is called,
    ///     so the strategy should still be able to resume accepting new tasks.
    /// </summary>
    protected abstract void WaitForCompletionInternal();

    /// <summary>
    ///     Resume accepting new tasks.
    ///     Should contain the logic to set up necessary states to accept new tasks.
    /// </summary>
    protected abstract void ResumeInternal();


    /// <summary>
    ///     Trigger finalization work for the _belongService.
    /// </summary>
    private void Final() {
        if (_state != ServiceStrategyState.Paused) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Paused,
                "Service Strategy is not paused.");
        }

        _belongService!.Final();
    }

    /// <summary>
    ///     Trigger the _belongService to work on the given data.
    /// </summary>
    /// <param name="data">Coming Data.</param>
    protected void WorkOnData(IServiceData<TService, object> data) {
        if (_state != ServiceStrategyState.Running) {
            throw new StrategyStateException(typeof(TService), _state, ServiceStrategyState.Running,
                "Service Strategy is not running.");
        }

        _belongService!.WorkOnData(data);
    }
}