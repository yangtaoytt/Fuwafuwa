using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;

namespace Fuwafuwa.Core.Distributor.Interface;

public interface IDistributor<TServiceData, TSubjectData, TSharedData>
    where TServiceData : IServiceData where TSubjectData : ISubjectData {
    int Distribute(int processorCount, TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);
}