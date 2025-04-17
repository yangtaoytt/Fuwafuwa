using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

public interface N_ICustomerHandler<TService, in TServiceData>
    where TService : IService<TService>
    where TServiceData : IServiceData<TService, TServiceData> {
    /// <summary>
    /// Handles the data.
    /// The function contains the specific logic to handle the data.
    /// </summary>
    /// <param name="data">The specific data.</param>
    void Handle(TServiceData data);
}