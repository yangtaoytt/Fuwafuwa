using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level0;
using Fuwafuwa.Core.Data.SharedDataWrapper.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level1;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;

namespace Fuwafuwa.Core.Container.Level3;

public sealed class
    ExecutorContainer<TExecutorCore, TServiceData, TSharedData, TInitData> : BaseContainerWithEmpty<TExecutorCore,
    ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>, TServiceData, NullSubjectData, TSharedData,
    TInitData, ExecutorService<TExecutorCore, TServiceData, TSharedData, TInitData>>
    where TServiceData : IServiceData
    where TExecutorCore : IExecutorCore<TServiceData, TSharedData, TInitData>, new()
    where TSharedData : ISharedDataWrapper {
    public ExecutorContainer(int serviceCount, DelSetDistribute setter, TInitData initData,
        Logger2Event? logger = null) :
        base(serviceCount, setter, initData, logger) { }
}