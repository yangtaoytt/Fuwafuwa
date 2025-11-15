using Fuwafuwa.Core.Core.Service.Handle;
using Fuwafuwa.Core.Core.Service.Others.Distributor;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Data;
/// <summary>
/// The abstract base class for processor service data.
/// The customer data can send to the service and get no result back.
/// The class implements the logic to find and call the corresponding service handler method,
/// and also implements the logic to return no result task to the caller.
/// </summary>
/// <typeparam name="TServiceData">Same as the IServiceData.</typeparam>
/// <typeparam name="TService">The corresponding service which implements the handle of this data.</typeparam>
public abstract class AConsumerData<TServiceData,TService> : AServiceData< TService,  TServiceData>
    where TService : IService<TService>, ICustomerHandler<TService, TServiceData> 
    where TServiceData : AConsumerData<TServiceData,TService >  {
    protected AConsumerData(IDistributor distributor) : base(distributor) { }

    public override void Accept(TService service) {
        service.Handle(Implement());
    }


    /// <summary>
    /// Sends the service data to the service with no result task returned.
    /// The reason you should pass the service to the data instead of passing the data to the service
    /// is that the data knows the result type.
    /// Otherwise, the service cannot return the correct result type.
    /// </summary>
    /// <param name="service">The reference of the corresponding service.</param>
    public void Send(TService service) {
        service.Receive(this);
    }
}
