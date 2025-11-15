using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategry;

public abstract class AServiceStrategy<TService> : IServiceStrategy<TService>
    where TService : AStrategyService<TService>{
    private AStrategyService<TService>? _belongService;
    public void Start(TService belongService) {
        _belongService = belongService;
        StartInternal();
    }
    protected abstract void StartInternal();

    public abstract void Receive(IServiceData<TService, object> serviceData);
    public abstract void ShutDown();

    protected void Final() {
        if (_belongService == null) {
            throw new NoServiceException();
        }
        _belongService.Final();
    }
    protected void WorkOnData(IServiceData<TService,object> data) {
        if (_belongService == null) {
            throw new NoServiceException();
        }
        _belongService.WorkOnData(data);
    }
}