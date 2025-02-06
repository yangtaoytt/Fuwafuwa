using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;

namespace Fuwafuwa.Core.Distributor.Interface;

public abstract class ISimpleDistributor<TServiceData, TSubjectData, TSharedData> : IDistributor <TServiceData, TSubjectData, TSharedData> where TSubjectData : ISubjectData where TServiceData : IServiceData {
    public int Distribute(int processorCount, TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData) {
        return Distribute(processorCount, serviceData, sharedData);
    }

    protected abstract int Distribute(int processorCount, TServiceData serviceData, TSharedData sharedData);
}