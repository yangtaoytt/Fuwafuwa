using Fuwafuwa.Core.Core.Service.Data;

namespace Fuwafuwa.Core.Core.Service.Service;

public interface IServiceReference;

/// <summary>
///     The base interface for all services.
/// </summary>
/// <typeparam name="TService">The subclass of this class.</typeparam>
public interface IService<out TService> : IServiceReference
    where TService : IService<TService> {
    /// <summary>
    ///     Receives the service data.
    ///     The function is called by the service data to deliver the data to the service.
    ///     You should not call this method directly when use a processor.
    ///     If not, you will not get the return value.
    /// </summary>
    /// <param name="serviceData">The data which is belonged to this service.</param>
    void Receive(IServiceData<TService, object> serviceData);

    /// <summary>
    ///     Start the service.
    ///     Should be called before using the service.
    /// </summary>
    TService Start();

    /// <summary>
    ///     Shut down the service and wait for all tasks to complete.
    /// </summary>
    void ShutDown();

    /// <summary>
    ///     Get the subclass instance reference.
    ///     The reference type is TService.
    ///     And it is the subclass type of the current class.
    ///     The reason to have this method is that we can make method chaining calling available.
    /// </summary>
    /// <returns>The subclass instance reference.</returns>
    TService Implement();
    
    TService WaitForCompletion();
    
    TService Resume();
}