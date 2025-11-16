using Fuwafuwa.Core.Core.Service.Others;

namespace Fuwafuwa.Core.Core.Service.Distributor;

/// <summary>
///     The distributor that distributes tasks in a polling manner.
/// </summary>
public class PollingDistributor : IDistributor {
    public ushort Distribute(DistributionData distributionData) {
        return (ushort)((distributionData.LastThreadId + 1) % distributionData.ThreadCount);
    }
}