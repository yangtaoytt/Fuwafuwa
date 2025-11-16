using Fuwafuwa.Core.Core.Service.Distributor;
using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.Core.Service.Service;

namespace Fuwafuwa.Core.Core.Service.Data;

/// <summary>
///     The abstract base class for service data.
///     It implements the IServiceData interface.
///     And make the Distribute method be implemented for all sub service data.
/// </summary>
/// <typeparam name="TService">Same as the IServiceData.</typeparam>
/// <typeparam name="TServiceData">Same as the IServiceData.</typeparam>
public abstract class AServiceData<TService, TServiceData> : IServiceData<TService, TServiceData>
    where TService : IService<TService> {
    private readonly IDistributor _distributor;

    protected AServiceData(IDistributor distributor) {
        _distributor = distributor;
    }

    public abstract void Accept(TService service);

    public ushort Distribute(DistributionData distributionData) {
        return _distributor.Distribute(distributionData);
    }

    public abstract TServiceData Implement();
}