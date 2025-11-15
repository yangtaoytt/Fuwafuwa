namespace Fuwafuwa.Core.Core.Service.Others.Distributor;

/// <summary>
///     The distributor that always distributes to the fixed thread (thread 0).
/// </summary>
internal class FixedDistributor : IDistributor {
    public ushort Distribute(DistributionData distributionData) {
        return 0;
    }
}