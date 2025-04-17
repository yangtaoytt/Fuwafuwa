using Fuwafuwa.Core.Core.Service.Others;
using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.New.Data;

public abstract class AServiceData< TService,  TServiceData> : IServiceData<TService,  TServiceData> 
    where TService : IService<TService>{
    private IDistributor _distributor;
    public AServiceData(IDistributor distributor) {
        _distributor = distributor;
    }

    public abstract void Accept(TService service);

    public ushort Distribute(DistributionData distributionData) {
        return _distributor.Distribute(distributionData);
    }

    public abstract TServiceData Implement();
}