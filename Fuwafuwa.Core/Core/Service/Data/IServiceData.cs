using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Data;
/// <summary>
/// The interface for service data.
/// The service data belongs to a specific service.
/// </summary>
/// <typeparam name="TService">the specific service.</typeparam>
/// <typeparam name="TServiceData">
/// The subclass of this interface.
/// The reason why not to limit the subclass by where stmt is to support
/// all kinds of data can be storage in the same channel of corresponding service without cast.
/// </typeparam>
 public interface IServiceData<in TService, out TServiceData>
    where TService : IService<TService> {
    /// <summary>
    /// Calls the service's corresponding method to handle the data.
    /// There can be multiple or zero service data belonging to the same service.
    /// But we need to automatically call the corresponding method of the service to handle the data.
    /// So we use the data to call the service's method.
    /// </summary>
    /// <param name="service">The service which the data belongs to.</param>*
    void Accept(TService service);
    
    /// <summary>
    /// Distributes the data according to the input data.
    /// The distribution should be decided by the service data but not the service.
    /// Because each service data may have different distribution logic.
    /// </summary>
    /// <param name="distributionData">The data of service distribution information.</param>
    /// <returns>The Thread Index of the chosen thread.</returns>
    ushort Distribute(DistributionData distributionData);

    /// <summary>
    /// Get the subclass instance reference.
    /// The reference type is TServiceData.
    /// And it is the subclass type of the current class.
    /// The reason to have this method is that the handler method should be implemented in the subclass
    /// which is defined by the user ,and we need to call it automatically on parent class.
    /// So we need to get the subclass instance reference here.
    /// </summary>
    /// <returns>The subclass instance reference.</returns>
    TServiceData Implement();
}