using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Distributor.Interface;

namespace Fuwafuwa.Core.Distributor.Implement;

public class
    PollingDistributor<TServiceData, TSubjectData, TSharedData> : IDistributor<TServiceData, TSubjectData, TSharedData>
    where TSubjectData : ISubjectData where TServiceData : IServiceData {
    private int _nextProcessorIndex;

    public int Distribute(int processorCount, TServiceData serviceData, TSubjectData subjectData,
        TSharedData sharedData) {
        ++_nextProcessorIndex;
        if (_nextProcessorIndex >= processorCount) {
            _nextProcessorIndex = 0;
        }

        return _nextProcessorIndex;
    }
}