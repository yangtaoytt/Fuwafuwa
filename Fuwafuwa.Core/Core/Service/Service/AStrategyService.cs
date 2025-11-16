using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

namespace Fuwafuwa.Core.Core.Service.Service;

/// <summary>
///     The abstract class for services with Strategy pattern.
///     Depends on IServiceStrategy to implement the actual thread logic.
/// </summary>
/// <typeparam name="TService">The subclass of this class.</typeparam>
public abstract class AStrategyService<TService> : IService<TService>
    where TService : AStrategyService<TService> {
    private readonly IServiceStrategy<TService> _strategy;

    private bool _hasStarted;

    protected AStrategyService(IServiceStrategy<TService> serviceStrategy) {
        _strategy = serviceStrategy;
        _hasStarted = false;
    }

    public void Receive(IServiceData<TService, object> serviceData) {
        if (!_hasStarted) {
            throw new ReceiveServiceDataBeforeStartException();
        }

        _strategy.Receive(serviceData);
    }

    public TService Start() {
        if (_hasStarted) {
            throw new StartServiceRepeatedlyException();
        }

        _hasStarted = true;
        _strategy.Start(Implement());
        return Implement();
    }

    public void ShutDown() {
        _strategy.ShutDown();
    }

    public abstract TService Implement();
    public TService WaitForCompletion() {
        _strategy.WaitForCompletion();
        return Implement();
    }
    public TService Resume() {
        _strategy.Resume();
        return Implement();
    }

    /// <summary>
    ///     Get called by the strategy to trigger the specific logic on the data.
    /// </summary>
    /// <param name="data">The data to handle.</param>
    public void WorkOnData(IServiceData<TService, object> data) {
        data.Accept(Implement());
    }

    /// <summary>
    ///     Do finalization work when the service is shutting down.
    /// </summary>
    public virtual void Final() { }
}