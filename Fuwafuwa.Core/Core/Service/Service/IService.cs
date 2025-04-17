using Fuwafuwa.Core.New.Data;

namespace Fuwafuwa.Core.New.Serviece;

public interface IServiceReference;


public interface IService<out TService> : IServiceReference
    where TService : IService<TService> {
    /// <summary>
    /// Receives the service data.
    /// </summary>
    /// <param name="serviceData">The data which is belonged to this service.</param>
    void Receive(IServiceData<TService,object> serviceData);
    
    /// <summary>
    /// Start the service.
    /// </summary>
    TService Start();
    
    TService Implement();
}
