using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

 public interface IServiceData<in TService, out TServiceData>
    where TService : IService<TService> {
    /// <summary>
    /// Accepts the service.
    /// Calls the service's corresponding method to handle the data.
    /// </summary>
    /// <param name="service">The service which the data belongs to.</param>*
    void Accept(TService service);
    
    /// <summary>
    /// Distributes the data according to the input data.
    /// </summary>
    /// <param name="distributionData">The data of service distribution information.</param>
    /// <returns>The Thread Index of the chosen thread.</returns>
    ushort Distribute(DistributionData distributionData);

    TServiceData Implement();
}