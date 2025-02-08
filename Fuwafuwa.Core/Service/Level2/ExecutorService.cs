using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Interface;
using Fuwafuwa.Core.Service.Level1;
using Fuwafuwa.Core.ServiceCore.Level3;

namespace Fuwafuwa.Core.Service.Level2;

public class
    ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData> : AServiceWithEmpty<TExecutorCore, TServiceData
        , NullSubjectData, TSharedData, TInitData, ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>,
        ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>>,
    IService<ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>, TSharedData, TInitData,
        ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>>
    where TServiceData : IServiceData
    where TExecutorCore : IExecutorCore<TServiceData, TSharedData, TInitData>, new()
    where TSharedData : ISharedDataWrapper {
    private ExecutorService(Logger2Event? logger) : base(logger) { }

    public static ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData> CreateService(
        Logger2Event? logger,
        ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        return new ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>(logger);
    }

    public static void Final(TSharedData sharedData, Logger2Event? logger,
        ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        TExecutorCore.Final(sharedData, logger);
    }

    public static TSharedData InitService(TInitData initData,
        ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>? uniqueService = null) {
        return TExecutorCore.Init(initData);
    }

    protected override Task ProcessData(TServiceData serviceData, NullSubjectData subjectData, TSharedData sharedData) {
        return ServiceCore.ExecuteTask(serviceData, sharedData, Logger);
    }
}