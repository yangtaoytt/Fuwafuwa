using Fuwafuwa.Core.New.Serviece;

namespace Fuwafuwa.Core.Core.Service.Others;

public class PollingDistributor : IDistributor {
    public ushort Distribute(DistributionData distributionData) {
        return (ushort)((distributionData.LastThreadId + 1) % distributionData.ThreadCount);
    }
}