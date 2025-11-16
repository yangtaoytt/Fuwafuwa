using Fuwafuwa.Core.Core.Service.Others;

namespace Fuwafuwa.Core.Core.Service.Distributor;

/// <summary>
///     The distributor that always distributes to the fixed thread (thread 0).
/// </summary>
public class FixedDistributor : IDistributor {
    public ushort Distribute(DistributionData distributionData) {
        return 0;
    }
}