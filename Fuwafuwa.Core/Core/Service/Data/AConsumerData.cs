using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

public abstract class AConsumerData<TServiceData,TService> : AServiceData< TService,  TServiceData>
    where TService : IService<TService>, N_ICustomerHandler<TService, TServiceData> 
    where TServiceData : AConsumerData<TServiceData,TService >  {
    protected AConsumerData(IDistributor distributor) : base(distributor) { }

    public override void Accept(TService service) {
        service.Handle(Implement());
    }


    /// <summary>
    /// Sends the service data to the service with no result task returned.
    /// </summary>
    /// <param name="service">The reference of the corresponding service.</param>
    public void Send(TService service) {
        service.Receive(this);
    }
}
