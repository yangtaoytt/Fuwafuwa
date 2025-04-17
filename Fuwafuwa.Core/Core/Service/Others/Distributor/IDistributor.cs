using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.Core.Service.Others;

public interface IDistributor {
    ushort Distribute(DistributionData distributionData);
}