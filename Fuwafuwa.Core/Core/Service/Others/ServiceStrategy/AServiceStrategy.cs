using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Others.ServiceStrategy;

/// <summary>
///     Handle the _belongService's lifecycle and data processing.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public abstract class AServiceStrategy<TService> : IServiceStrategy<TService>
    where TService : AStrategyService<TService> {
    private AStrategyService<TService>? _belongService;

    public void Start(TService belongService) {
        _belongService = belongService;
        StartInternal();
    }

    public abstract void Receive(IServiceData<TService, object> serviceData);
    public abstract void ShutDown();

    /// <summary>
    ///     Internal start method called after setting the belong service.
    /// </summary>
    protected abstract void StartInternal();

    /// <summary>
    ///     Trigger finalization work for the _belongService.
    /// </summary>
    protected void Final() {
        if (_belongService == null) {
            throw new NoServiceException();
        }

        _belongService.Final();
    }

    /// <summary>
    ///     Trigger the _belongService to work on the given data.
    /// </summary>
    /// <param name="data">Coming Data.</param>
    protected void WorkOnData(IServiceData<TService, object> data) {
        if (_belongService == null) {
            throw new NoServiceException();
        }

        _belongService.WorkOnData(data);
    }
}