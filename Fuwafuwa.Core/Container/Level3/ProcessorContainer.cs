using Fuwafuwa.Core.Container.Level2;
using Fuwafuwa.Core.Data.ServiceData.Level1;
using Fuwafuwa.Core.Data.SharedDataWapper.Implement;
using Fuwafuwa.Core.Data.SharedDataWapper.Level0;
using Fuwafuwa.Core.Data.SubjectData.Level2;
using Fuwafuwa.Core.Log;
using Fuwafuwa.Core.Service.Level2;
using Fuwafuwa.Core.ServiceCore.Level3;
using Fuwafuwa.Core.ServiceRegister;

namespace Fuwafuwa.Core.Container.Level3;

public sealed class
    ProcessorContainer<TProcessorCore, TServiceData, TSharedData, TInitData> : BaseContainerWithRegister<TProcessorCore,
    ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>, TServiceData,
    SubjectDataWithCommand, TSharedData, TInitData,ProcessService<TProcessorCore, TServiceData, TSharedData, TInitData>>
    where TServiceData : IProcessorData
    where TProcessorCore : IProcessorCore<TServiceData, TSharedData, TInitData>, new()
    where TSharedData : ISharedDataWrapper {
    public ProcessorContainer(int serviceCount, DelSetDistribute setter,
        (SimpleSharedDataWrapper<Register>, TInitData) initData, Logger2Event? logger = null) : base(serviceCount,
        setter, initData, logger) { }
}