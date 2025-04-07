using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

interface N_ICustomerHandle<TService, in TServiceData>
    where TService : N_IService<TService>
    where TServiceData : N_IServiceData<TService, TServiceData> {
    /// <summary>
    /// Handles the data.
    /// The function contains the specific logic to handle the data.
    /// </summary>
    /// <param name="data">The specific data.</param>
    void Handle(TServiceData data);
}