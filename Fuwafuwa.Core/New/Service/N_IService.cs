using Fuwafuwa.Core.New.Data;

namespace Fuwafuwa.Core.New.Serviece;

interface N_IService<out TService>
    where TService : N_IService<TService> {
    /// <summary>
    /// Receives the service data.
    /// </summary>
    /// <param name="serviceData">The data which is belonged to this service.</param>
    void Receive(N_IServiceData<TService,object> serviceData);
    
    /// <summary>
    /// Start the service.
    /// </summary>
    void Start();
    
    protected TService Implement();
}
