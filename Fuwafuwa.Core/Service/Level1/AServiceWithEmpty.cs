using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Service.Level0;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithEmpty<TServiceData, TSubjectData, TSharedData> : AService<TServiceData, TSubjectData,
    InitTuple<TSharedData>> where TSubjectData : ISubjectData
    where TServiceData : IServiceData
    where TSharedData : new() {
    protected override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        InitTuple<TSharedData> initTuple) {
        return ProcessData(serviceData, subjectData, initTuple.Item1);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);
}