using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Distributor.Interface;

namespace Fuwafuwa.Core.Distributor.Implement;

public class
    HashDistributor<TServiceData, TSubjectData, TSharedData> : IDistributor<TServiceData, TSubjectData, TSharedData>
    where TServiceData : IServiceData where TSubjectData : SubjectData {
    public int Distribute(int processorCount, TServiceData serviceData, TSubjectData subjectData,
        TSharedData sharedData) {
        var subjectId = subjectData.Subject.UniqueId;

        return (int)(subjectId % processorCount);
    }
}