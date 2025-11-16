using Fuwafuwa.Core.Core.Service.Others;

namespace Fuwafuwa.Core.Core.Service.Distributor;

/// <summary>
///     The interface for thread distributors.
/// </summary>
public interface IDistributor {
    ushort Distribute(DistributionData distributionData);
}