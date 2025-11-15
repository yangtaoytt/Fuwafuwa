namespace Fuwafuwa.Core.Core.Service.Others.Distributor;

/// <summary>
/// The distributor that distributes tasks in a polling manner.
/// </summary>
public class PollingDistributor : IDistributor {
    public ushort Distribute(DistributionData distributionData) {
        return (ushort)((distributionData.LastThreadId + 1) % distributionData.ThreadCount);
    }
}