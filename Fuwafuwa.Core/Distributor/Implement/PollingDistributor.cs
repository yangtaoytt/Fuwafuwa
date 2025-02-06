using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Distributor.Interface;

namespace Fuwafuwa.Core.Distributor.Implement;

public class
    PollingDistributor<TServiceData, TSubjectData, TSharedData> : ISimpleDistributor<TServiceData, TSubjectData, TSharedData>
    where TSubjectData : ISubjectData where TServiceData : IServiceData {
    private int _nextProcessorIndex;
    protected override int Distribute(int processorCount, TServiceData serviceData, TSharedData sharedData) {
        ++_nextProcessorIndex;
        if (_nextProcessorIndex >= processorCount) {
            _nextProcessorIndex = 0;
        }

        return _nextProcessorIndex;
    }
}