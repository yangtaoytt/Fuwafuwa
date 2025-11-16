using Fuwafuwa.Core.Core.Service.Data;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.ServiceStrategy;

/// <summary>
///     The interface for service strategies.
///     Defines how a service processes on threads.
/// </summary>
/// <typeparam name="TService">The corresponding Service Type.</typeparam>
public interface IServiceStrategy<TService> where TService : AStrategyService<TService> {
    /// <summary>
    ///     Start the strategy with the belong service.
    ///     Do any initialization here.
    /// </summary>
    /// <param name="belongService">Save the service with this parameter.</param>
    void Start(TService belongService);

    /// <summary>
    ///     Define What to do when receiving service data.
    /// </summary>
    /// <param name="serviceData">The coming data.</param>
    void Receive(IServiceData<TService, object> serviceData);

    /// <summary>
    ///     Shut down the threads.
    /// </summary>
    void ShutDown();

    /// <summary>
    ///     Stop accepting new tasks and wait for all running tasks to complete.
    /// </summary>
    void WaitForCompletion();

    /// <summary>
    ///     Resume accepting new tasks.
    /// </summary>
    void Resume();
}