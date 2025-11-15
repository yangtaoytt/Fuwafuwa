using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategry;

public interface IServiceStrategy<TService> where TService : AStrategyService<TService> {
    void Start(TService belongService);
    void Receive(IServiceData<TService,object> serviceData);
    
    void ShutDown();
}