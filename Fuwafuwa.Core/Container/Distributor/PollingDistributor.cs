using Fuwafuwa.Core.Data.PrimaryInfo.Interface;

namespace Fuwafuwa.Core.Container.Distributor;

public class PollingDistributor<TPrimaryInfo> : IDistributor<TPrimaryInfo> where TPrimaryInfo : IPrimaryInfo {
    private int _nextProcessorIndex;

    public int Distribute(int processorCount, TPrimaryInfo info) {
        ++_nextProcessorIndex;
        if (_nextProcessorIndex >= processorCount) {
            _nextProcessorIndex = 0;
        }

        return _nextProcessorIndex;
    }
}