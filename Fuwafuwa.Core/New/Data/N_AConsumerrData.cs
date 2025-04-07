using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

abstract class N_AConsumerrData<TService, TServiceData> : N_IServiceData<TService,TServiceData>
    where TService : N_IService<TService>, N_ICustomerHandle<TService, TServiceData> 
    where TServiceData : N_AConsumerrData<TService, TServiceData>  {
    public void Accept(TService service) {
        service.Handle(Implement());
    }

    public abstract ushort Distribute(N_DistributionData distributionData);
    public abstract TServiceData Implement();

    /// <summary>
    /// Sends the service data to the service with no result task returned.
    /// </summary>
    /// <param name="service">The reference of the corresponding service.</param>
    public void Send(TService service) {
        service.Receive(this);
    }
}
