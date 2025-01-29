using Fuwafuwa.Core.Data.InitTuple;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;

namespace Fuwafuwa.Core.Service.Level2;

public class
    ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData> : AServiceWithEmpty<TExecutorCore, TServiceData
    , NullSubjectData, TSharedData, TInitData>
    where TSharedData : new()
    where TServiceData : IServiceData
    where TExecutorCore : IExecutorCore<TServiceData, TSharedData, TInitData>, new() {
    protected override Task ProcessData(TServiceData serviceData, NullSubjectData subjectData, TSharedData sharedData, Lock sharedDataLock) {
        return ServiceCore.ExecuteTask(serviceData, sharedData,sharedDataLock, Logger);
    }

    protected override TSharedData SubInit(TInitData initData) {
        return TExecutorCore.Init(initData);
    }


    public override void Final(InitTuple<TSharedData> sharedData, Logger2Event? logger) {
        base.Final(sharedData, logger);
        TExecutorCore.Final(sharedData.Item1, logger);
    }
}