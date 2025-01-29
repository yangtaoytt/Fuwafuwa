using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level0;
using Fuwafuwa.Core.Service.Level0;
using Fuwafuwa.Core.ServiceCore.Level0;

namespace Fuwafuwa.Core.Service.Level1;

public abstract class
    AServiceWithEmpty<TServiceCore, TServiceData, TSubjectData, TSharedData, TInitData> : AService<TServiceCore,
    TServiceData, TSubjectData,
    InitTuple<TSharedData>, TInitData> where TSubjectData : ISubjectData
    where TServiceData : IServiceData
    where TSharedData : new()
    where TServiceCore : IServiceCore<TServiceData>, new() {
    protected override Task ProcessData(TServiceData serviceData, TSubjectData subjectData,
        InitTuple<TSharedData> initTuple) {
        return ProcessData(serviceData, subjectData, initTuple.Item1);
    }

    protected abstract Task ProcessData(TServiceData serviceData, TSubjectData subjectData, TSharedData sharedData);

    protected override InitTuple<TSharedData> Init(TInitData initData) {
        return new InitTuple<TSharedData>(SubInit(initData));
    }

    protected abstract TSharedData SubInit(TInitData initData);
}