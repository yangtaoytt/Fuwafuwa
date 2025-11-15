using System.Threading.Channels;
using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.ServiceStrategry;
using Fuwafuwa.Core.Logger;

namespace Fuwafuwa.Core.Core.Service.Service;

/// <summary>
/// The abstract class for services with static number of threads.
/// The number of threads is defined when constructing the service.
/// The service will create the threads when started.
/// And each thread will have its own channel to receive the service data.
/// The main thread will distribute the data to each sub thread according to the distribution logic defined in the service data.
/// The service data will be processed in the sub threads.
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
    
    public void Receive(IServiceData<TService,object> serviceData) {
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

    public void WorkOnData(IServiceData<TService,object> data) {
        data.Accept(Implement());
    }

    public virtual void Final() { }
}