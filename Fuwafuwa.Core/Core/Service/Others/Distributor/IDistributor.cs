namespace Fuwafuwa.Core.Core.Service.Others.Distributor;

/// <summary>
/// The interface for thread distributors.
/// </summary>
public interface IDistributor {
    ushort Distribute(DistributionData distributionData);
}